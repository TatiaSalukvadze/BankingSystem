using BankingSystem.Contracts.Interfaces.IExternalServices;
using BankingSystem.Infrastructure.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
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
    public class ExchangeRateService : IExchangeRateService
    {
        private readonly CurrencyApiSettings _currencyApiSettings;
        //private readonly string _apiKey = "2cd5220a1105f2545898dec2";
        //private readonly string _baseUrl = "https://v6.exchangerate-api.com/v6/";
        private readonly HttpClient _httpClient;
        public ExchangeRateService(HttpClient httpClient, IOptions<CurrencyApiSettings> currencyApiSettings) {
            _httpClient = httpClient;
            _currencyApiSettings = currencyApiSettings.Value;
        }

        public async Task<decimal> GetCurrencyRateAsync(string fromCurrency, string toCurrency)
        {
            string url = _currencyApiSettings.BaseUrl + _currencyApiSettings.ApiKey + "/latest/" + fromCurrency;
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
