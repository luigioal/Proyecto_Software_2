using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AppLogic.ConnectorsAdmin
{
    public class FinanceConnector
    {
        private const string API_Key = "XzFe20z0QYPx9ERuPNernOO5zMgDGuJ5";

        public double GetCurrentPrice(string symbol)
        {
            HttpClient client = new HttpClient();
            string requestUrl = $"https://financialmodelingprep.com/api/v3/profile/{symbol}?apikey={API_Key}";

            try
            {
                var response = client.GetAsync(requestUrl).Result;

                if (response.IsSuccessStatusCode)
                {
                    var jsonData = response.Content.ReadAsStringAsync().Result;
                    var result = JsonConvert.DeserializeObject<List<CompanyProfile>>(jsonData);

                    if (result != null && result.Count > 0)
                    {
                        return (double)result[0].Price;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error here
                Console.WriteLine($"Error fetching price: {ex.Message}");
            }
            return 0.0;
        }
    }

    // Supporting class to deserialize the JSON response
    public class CompanyProfile
    {
        [JsonProperty("price")]
        public decimal Price { get; set; }

        // You can add other properties you need from the response
        // [JsonProperty("symbol")]
        // public string Symbol { get; set; }
    }
}
