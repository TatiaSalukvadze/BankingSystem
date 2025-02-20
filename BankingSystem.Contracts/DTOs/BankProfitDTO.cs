using BankingSystem.Domain.Enums;

namespace BankingSystem.Contracts.DTOs
{
    public class BankProfitDTO
    {
        public string Currency { get; set; }
        public decimal LastMonthProfit { get; set; }
        public decimal LastSixMonthProfit { get; set; }
        public decimal LastYearProfit { get; set; }
    }
}
