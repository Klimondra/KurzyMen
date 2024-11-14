using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace APIRequestExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            double? eurDevStred = await GetEurDevStred();
            if (eurDevStred.HasValue)
            {
                Console.WriteLine($"Devizový střed EUR: {eurDevStred.Value}");
            }
            else
            {
                Console.WriteLine("Nepodařilo se načíst kurz EUR.");
            }
        }

        public static async Task<double?> GetEurDevStred()
        {
            string url = "https://data.kurzy.cz/json/meny/b[6]cb[vypsat].js";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    string responseBody = await response.Content.ReadAsStringAsync();

                    // Odebereme neplatné části "vypsat(" a ");" z JSON odpovědi
                    if (responseBody.StartsWith("vypsat(") && responseBody.EndsWith(");"))
                    {
                        responseBody = responseBody.Substring(7, responseBody.Length - 9);
                    }

                    // Deserializace JSON odpovědi a načtení hodnoty dev_stred pro EUR
                    using (JsonDocument doc = JsonDocument.Parse(responseBody))
                    {
                        JsonElement root = doc.RootElement;
                        JsonElement kurzy = root.GetProperty("kurzy");
                        JsonElement eur = kurzy.GetProperty("EUR");
                        double eurDevStred = eur.GetProperty("dev_stred").GetDouble();
                        return eurDevStred;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Chyba: {e.Message}");
                return null;
            }
        }
    }
}
