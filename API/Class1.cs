using System.Text.Json;
using System.Net.Http;
namespace API
{
    public class GetAPIData
    {
        //hiermee kan de httpcall gedaan worden
        private HttpClient client = new HttpClient();

        public GetAPIData()
        {
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "c94acbf3136844b08c1fa4281955241b");
        }
        public async Task<Dictionary<string,string>> getAPIURLs()
        {
            Dictionary<string, string> d = new Dictionary<string, string>();
            string url = "https://api.delijn.be/DLKernOpenData/api/v1/api";
            string response = await client.GetStringAsync(url);
            using(JsonDocument document = JsonDocument.Parse(response))
            {
                JsonElement root = document.RootElement;
                JsonElement links = root.GetProperty("links");
                foreach(JsonElement link in links.EnumerateArray())
                {
                    d.Add(link.GetProperty("rel").ToString(), link.GetProperty("url").ToString());
                }
            }
            return d;
        }
    }
}
