using BankingSystem.Contracts.Exceptions;
using BankingSystem.Contracts.Interfaces.IExternalServices;
using BankingSystem.Infrastructure.ExternalServices.Configuration;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;

namespace BankingSystem.Infrastructure.ExternalServices
{
    public class ExchangeRateService : IExchangeRateService
    {
        private readonly CurrencyApiSettings _currencyApiSettings;
        private readonly HttpClient _httpClient;

        public ExchangeRateService(HttpClient httpClient, IOptions<CurrencyApiSettings> currencyApiSettings) 
        {
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
                throw new CurrencyRateNotFoundException($"Currency rate could not be found for {toCurrency}");
            }
        }
    }
}
//Example Request: https://v6.exchangerate-api.com/v6/2cd5220a1105f2545898dec2/latest/USD