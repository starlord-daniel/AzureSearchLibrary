using AzureSearch.Api;
using AzureSearch.Helper;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AzureSearch.Dialogs
{
    [Serializable]
    public class SearchDialog
    {
        private SearchApi _searchApi;
        private SqlConnector _sqlConnector;
        private float _searchThreshold;

        public SearchDialog(SearchApi searchApi, SqlConnector sqlConnector, float searchThreshold = 0.5f)
        {
            _searchApi = searchApi;
            _sqlConnector = sqlConnector;
            _searchThreshold = searchThreshold;
        }

        public async Task StartAzureSearchDialogAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            string question = activity.Text;

            // Get Azure Search result
            var searchResult = await _searchApi.GetAnswerAsync(question);
            var answers = searchResult.result;

            if (answers == null)
            {
                await context.PostAsync(searchResult.validationResult);
            }
            else
            {
                if (answers.value.Length < 1)
                {
                    // tell the user, that we couldn't find an answer
                    await context.PostAsync("Unfortunately, I was unable to find an answer for your question. Please try again, by asking another one.");
                }
                else
                {
                    // Check, if score is sufficient
                    var score = answers.value.Length > 0 ? answers.value.First().searchscore : -1;

                    if (score < _searchThreshold)
                    {
                        // if score is unsufficient -> send error message
                        var m = context.MakeMessage();

                        await context.PostAsync(m);
                    }
                    else
                    {
                        var answer = answers.value.First().answer;

                        if (answer != null)
                        {
                            // Case 1: Display answer
                            var m = context.MakeMessage();

                            await context.PostAsync(m);
                        }
                        else
                        {
                            // Case 2: Ask for follow up
                            var followUpIndex = answers.value.First().followup;

                            await FollowUpBegin(context, followUpIndex);
                        }
                    }
                }
            }            
        }

        private async Task FollowUpBegin(IDialogContext context, int followUpIndex)
        {
            var followUpResultList = await _sqlConnector.GetFollowUpResultAsync(followUpIndex);     // Let the user select the column names for the sql table
            var followUpResult = followUpResultList[0].result;

            if (followUpResult.FollowUp == "")
            {
                var finalMessage = context.MakeMessage();
                finalMessage.Text = followUpResult.Response;
                finalMessage.Speak = followUpResult.Response;

                await context.PostAsync(finalMessage);
            }
            else
            {
                var options = BotHelper.GenerateListFromString(followUpResult.Options);
                var followUps = BotHelper.GenerateListFromString(followUpResult.FollowUp);

                context.ConversationData.SetValue("cFollowUp", followUps);
                context.ConversationData.SetValue("cOptions", options);

                var promptOptions = new PromptOptionsWithSynonyms<string>(
                    prompt: followUpResult.Response,
                    retry: "Unfortunately this option is not available",
                    choices: BotHelper.ConvertListToDict(options),
                    speak: followUpResult.Response,
                    retrySpeak: "Unfortunately this option is not available"
                    );

                PromptDialog.Choice(
                    context,
                    FollowUpResumeAfter,
                    promptOptions
                    );
            }
        }

        private async Task FollowUpResumeAfter(IDialogContext context, IAwaitable<string> result)
        {
            var answer = await result;

            var followUpList = context.ConversationData.GetValue<List<string>>("cFollowUp");
            var optionList = context.ConversationData.GetValue<List<string>>("cOptions");

            var followUpIndex = Convert.ToInt32(followUpList[optionList.IndexOf(answer)]);

            await FollowUpBegin(context, followUpIndex);
        }

        
    }
}
