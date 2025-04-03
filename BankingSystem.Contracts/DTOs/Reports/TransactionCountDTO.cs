using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Contracts.DTOs.Report
{
    public class TransactionCountDTO
    {
        [Required]
        public int LastMonthCount { get; set; }

        [Required]
        public int LastSixMonthCount { get; set; }

        [Required]
        public int LastYearCount { get; set; }
    }
}
