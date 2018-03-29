using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using AzureSearch.Api;
using AzureSearch.Dialogs;
using System.Web.Configuration;

namespace AzureSearchDialogBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            SearchApi searchApi = new SearchApi(
                WebConfigurationManager.AppSettings["AzureSearchName"],
                WebConfigurationManager.AppSettings["AzureSearchKey"],
                WebConfigurationManager.AppSettings["AzureSearchIndexName"]
                );

            SqlConnector sqlApi = new SqlConnector(
                WebConfigurationManager.AppSettings["SqlConnectionString"],
                WebConfigurationManager.AppSettings["SqlTableName"]
                );

            SearchDialog searchDialog = new SearchDialog(
                searchApi: searchApi, 
                sqlConnector: sqlApi, 
                searchThreshold: 0.5f
                );

            // Forward the users input to Azure Search dialog
            context.Wait(searchDialog.StartAzureSearchDialogAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            context.Wait(MessageReceivedAsync);
        }
    }
}