using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Contracts.Interfaces.IExternalServices
{
    public interface IExchangeRateService
    {
        Task<decimal> GetCurrencyRateAsync(string fromCurrency, string toCurrency);
    }
}
