using AzureSearch.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AzureSearch.Api
{
    public class SearchApi
    {
        private string _searchName;
        private string _searchKey;
        private string _indexName;
        private string _apiVersion;

        /// <summary>
        /// To use the Azure Search functionality, instantiate this class, provide the credentials and call the given methods. To learn more, visit: https://docs.microsoft.com/en-us/azure/search/search-query-rest-api
        /// </summary>
        /// <param name="searchName">The Azure Search name. Format: searchName.search.windows.net </param>
        /// <param name="searchKey">The Azure Search key.</param>
        /// <param name="indexName">The name of your Azure Search index.</param>
        /// <param name="apiVersion">The api version of the Azure Search service. It defaults to "2016-09-01".</param>
        public SearchApi(string searchName, string searchKey, string indexName, string apiVersion = "2016-09-01")
        {
            _searchName = searchName;
            _searchKey = searchKey;
            _indexName = indexName;
            _apiVersion = apiVersion;
        }

        /// <summary>
        /// A simple call for the results of an Azure Search query.. To learn more, visit: https://docs.microsoft.com/en-us/azure/search/search-query-rest-api
        /// </summary>
        /// <param name="question">The question from the user, that is send to Azure Search.</param>
        /// <returns>Tuple (AzureSearchResult, validationResult). AzureSearchResult is a list of answers with metadata from Azure Search. validationResult is a string which contains an error message to display</returns>
        public async Task<(AzureSearchResult result, string validationResult)> GetAnswerAsync(string question)
        {
            var validation = ValidateCredentials();

            if( !validation.validated )
            {
                return (null, validation.result);
            }

            string queryString = $"https://{_searchName}/indexes/{_indexName}/docs?&api-version={_apiVersion}&";

            HttpResponseMessage resultHttpResponse = new HttpResponseMessage();
            using ( var httpClient = new HttpClient() )
            {
                HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, queryString + "search=" + question);
                httpRequestMessage.Headers.Add("host", _searchName);
                httpRequestMessage.Headers.Add("Api-key", _searchKey);
                resultHttpResponse = await httpClient.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseContentRead);
            }

            var output = await resultHttpResponse.Content.ReadAsStringAsync();
            return (JsonConvert.DeserializeObject<AzureSearchResult>(output), "valid");
        }

        /// <summary>
        /// Validates the credentials for the Azure Search call.
        /// </summary>
        /// <returns></returns>
        private (string result, bool validated) ValidateCredentials()
        {
            List<string> credentialList = new List<string> { _searchName, _searchKey, _indexName };
            Regex searchNamePattern = new Regex(".*.search.windows.net");

            if (credentialList.All(x => string.IsNullOrEmpty(x)))
            {
                return ("One of the arguments is null or empty. Please provide them in the web.config file.", false);
            }
            else if(!searchNamePattern.IsMatch(_searchName))
            {
                return ($"The given search name: {_searchName} is not in the format: <name>.search.windows.net", false);
            }

            return ("valid", true);
        }
    }
}
