using System.Text.Json;
using System.Net.Http;
namespace API
{
    public class GetAPIData
    {
        //hiermee kan de httpcall gedaan worden
        private HttpClient client = new HttpClient();
        //hierin slaan we alle gemeentes op
        public Dictionary<string, int> gemeentes = new Dictionary<string ,int>();
        ///<summary>
        ///de start-up dat zorgt dat de key wordt in de header gestoken en de dictionaries worden ingeladen
        ///</summary>
        public GetAPIData()
        {
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "c94acbf3136844b08c1fa4281955241b");
            refreshGemeentes();
        }

        ///<summary>
        ///geeft de urls terug in dictionary met location als key en url als value
        ///</summary>
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
        ///<summary>
        ///herlaad de gegevens in Gemeentes
        ///</summary>
        private async void refreshGemeentes()
        {
            string url = "https://api.delijn.be/DLKernOpenData/api/v1/gemeenten";
            string response = await client.GetStringAsync(url);
            using (JsonDocument document = JsonDocument.Parse(response))
            {
                JsonElement root = document.RootElement;
                JsonElement gemeenten = root.GetProperty("gemeenten");
                foreach(JsonElement gemeente in gemeenten.EnumerateArray())
                {
                    try
                    {
                        if (gemeente.GetProperty("hoofdGemeente").ToString() != null) {
                            if (gemeente.GetProperty("omschrijving").ToString() != gemeente.GetProperty("hoofdGemeente").GetProperty("omschrijving").ToString())
                                gemeentes.Add(gemeente.GetProperty("omschrijving").ToString(), Convert.ToInt32(gemeente.GetProperty("gemeentenummer").ToString()));
                        }
                    }
                    catch (KeyNotFoundException)
                    {
                        try {
                            gemeentes.Add(gemeente.GetProperty("omschrijving").ToString(), Convert.ToInt32(gemeente.GetProperty("gemeentenummer").ToString()));
                        }
                        catch(Exception)
                        {

                        }
                        }
                    catch (Exception)
                    {

                    }
                }
                gemeentes = gemeentes.OrderBy(pair=>pair.Value).ToDictionary();
            }
        }
    }
}
