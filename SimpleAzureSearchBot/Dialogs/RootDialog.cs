using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;
using AzureSearch.Api;
using AzureSearch.Model;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace SimpleAzureSearchBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            // Declare Azure search object
            SearchApi search = new SearchApi(
                WebConfigurationManager.AppSettings["AzureSearchName"],
                WebConfigurationManager.AppSettings["AzureSearchKey"],
                WebConfigurationManager.AppSettings["AzureSearchIndexName"]
                );

            // Get Azure Search result
            var searchResult = await search.GetAnswerAsync(activity.Text);
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
                    // return the first Azure Search answer to the user
                    await context.PostAsync(answers.value.First().answer);
                }
            }

            // Close the loop by waiting for an answer and sending it to the MessageReceivedAsync method
            context.Wait(MessageReceivedAsync);
        }
    }
}