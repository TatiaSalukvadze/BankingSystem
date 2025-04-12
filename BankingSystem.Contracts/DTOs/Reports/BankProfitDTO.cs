using BankingSystem.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Contracts.DTOs.Report
{
    public class BankProfitDTO
    {
        public CurrencyType Currency { get; set; }

        public decimal LastMonthProfit { get; set; }

        public decimal LastSixMonthProfit { get; set; }

        public decimal LastYearProfit { get; set; }
    }
}
