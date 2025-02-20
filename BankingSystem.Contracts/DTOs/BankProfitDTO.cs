using BankingSystem.Domain.Enums;

namespace BankingSystem.Contracts.DTOs
{
    public class BankProfitDTO
    {
        public CurrencyType Currency { get; set; }
        public decimal LastMonthProfit { get; set; }
        public decimal LastSixMonthProfit { get; set; }
        public decimal LastYearProfit { get; set; }
    }
}
