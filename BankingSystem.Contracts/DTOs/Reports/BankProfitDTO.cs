using BankingSystem.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Contracts.DTOs.Report
{
    public class BankProfitDTO
    {
        [Required]
        public CurrencyType Currency { get; set; }

        [Required]
        public decimal LastMonthProfit { get; set; }

        [Required]
        public decimal LastSixMonthProfit { get; set; }

        [Required]
        public decimal LastYearProfit { get; set; }
    }
}
