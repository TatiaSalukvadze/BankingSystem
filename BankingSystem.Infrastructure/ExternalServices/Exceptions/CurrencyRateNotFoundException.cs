namespace BankingSystem.Infrastructure.Exceptions
{
    public class CurrencyRateNotFoundException : Exception
    {
        public CurrencyRateNotFoundException() { }
        public CurrencyRateNotFoundException(string message) : base(message) { }
        public CurrencyRateNotFoundException(string message, Exception inner) : base(message, inner) { }
    }
}
