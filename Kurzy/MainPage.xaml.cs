using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;


namespace Kurzy
{
    public partial class MainPage : ContentPage
    {
        private static string mena = "EUR";

        public MainPage()
        {
            InitializeComponent();
            LoadEurDevStred();

            nadpisEur.Text = $"Kurzy z {mena}";
            podnadpisEur.Text = $"{mena}";
            eurlabel.Text = $"0 {mena}";
        }

        private async void LoadEurDevStred()
        {
            double eurDevStred = await GetEurDevStred();
            if (eurDevStred != 0)
            {
                Input1.Text = eurDevStred.ToString();
            }
            else
            {
                Input1.Text = "1";
            }
        }

        private async void Button_OnClicked(object? sender, EventArgs e)
        {
            double castka1 = 0;
            if (!double.TryParse(Input1.Text, out castka1))
            {
                eurlabel.Text = "Částka není číslo!";
            }
            else
            {
                double eurDevStred = await GetEurDevStred();
                if (eurDevStred != 0)
                {
                    eurlabel.Text = $"{Math.Round(castka1 * (1 / eurDevStred), 2)} {mena}";
                }
                else
                {
                    Console.WriteLine($"Nepodařilo se načíst kurz {mena}.");
                }
            }
        }
        
        private async void Button2_OnClicked(object? sender, EventArgs e)
        {
            double castka1 = 0;
            if (!double.TryParse(Input2.Text, out castka1))
            {
                eurlabel.Text = "Částka není číslo!";
            }
            else
            {
                double eurDevStred = await GetEurDevStred();
                if (eurDevStred != 0)
                {
                    czklabel.Text = $"{Math.Round(castka1 * eurDevStred, 2)} CZK";
                }
                else
                {
                    Console.WriteLine($"Nepodařilo se načíst kurz {mena}.");
                }
            }
        }

        public static async Task<double> GetEurDevStred()
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
                        JsonElement eur = kurzy.GetProperty(mena);
                        double eurDevStred = eur.GetProperty("dev_stred").GetDouble();
                        return eurDevStred;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Chyba: {e.Message}");
                return 0;
            }
        }


        private void btnSwitch_clicked(object? sender, EventArgs e)
        {
            switch (btnSwitch.Text)
            {
                case "Změna na USD":
                    mena = "USD";
                    btnSwitch.Text = "Změna na EUR";
                    
                    nadpisEur.Text = $"Kurzy z {mena}";
                    podnadpisEur.Text = $"{mena}";
                    eurlabel.Text = $"0 {mena}";
                    LoadEurDevStred();
                    break;
                
                case "Změna na EUR":
                    mena = "EUR";
                    btnSwitch.Text = "Změna na USD";
                    
                    nadpisEur.Text = $"Kurzy z {mena}";
                    podnadpisEur.Text = $"{mena}";
                    eurlabel.Text = $"0 {mena}";
                    LoadEurDevStred();
                    break;
            }
        }
    }
}
