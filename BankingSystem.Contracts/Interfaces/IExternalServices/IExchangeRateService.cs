namespace BankingSystem.Contracts.Interfaces.IExternalServices
{
    public interface IExchangeRateService
    {
        Task<decimal> GetCurrencyRateAsync(string fromCurrency, string toCurrency);
    }
}
