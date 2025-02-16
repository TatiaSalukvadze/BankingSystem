using BankingSystem.Contracts.Interfaces.IExternalServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BankingSystem.Infrastructure.ExternalServices
{
    public class CurrencyService : ICurrencyService
    {
        private readonly string _apiKey = "2cd5220a1105f2545898dec2";
        private readonly string _baseUrl = "https://v6.exchangerate-api.com/v6/";
        private readonly HttpClient _httpClient;
        public CurrencyService(HttpClient httpClient) {
            _httpClient = httpClient;
        }

        public async Task<decimal> GetCurrencyRate(string fromCurrency, string toCurrency)
        {
            string url = _baseUrl + _apiKey + "/latest/" + fromCurrency;
            var result = await _httpClient.GetFromJsonAsync<JsonElement>(url);
            if(result.TryGetProperty("conversion_rates", out JsonElement conversionRates) 
                && conversionRates.TryGetProperty(toCurrency, out JsonElement currencyRate)
                && currencyRate.TryGetDecimal(out decimal rate))
            {
                return rate;
            }
            else
            {
                throw new Exception($"Currency rate could not be found for {toCurrency}");
            }

        }
//            Your API Key: 2cd5220a1105f2545898dec2
//Example Request: https://v6.exchangerate-api.com/v6/2cd5220a1105f2545898dec2/latest/USD
    }
}
