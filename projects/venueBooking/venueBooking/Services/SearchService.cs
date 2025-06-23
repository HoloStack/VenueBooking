using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;

namespace venueBooking.Services
{
    public class SearchService
    {
        private readonly SearchClient _client;

        public SearchService(IConfiguration config)
        {
            var endpoint = new Uri($"https://{config["AzureSearch:ServiceName"]}.search.windows.net");
            var key = new AzureKeyCredential(config["AzureSearch:ApiKey"]);
            _client = new SearchClient(endpoint, config["AzureSearch:IndexName"], key);
        }

        public async Task<SearchResults<object>> SearchAsync(string term)
        {
            return await _client.SearchAsync<object>(term);
        }
    }
}