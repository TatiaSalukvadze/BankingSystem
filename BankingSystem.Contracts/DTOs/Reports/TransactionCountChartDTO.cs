using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Contracts.DTOs.Report
{
    public class TransactionCountChartDTO
    {
        [Required]
        public DateOnly Date { get; set; }

        [Required]
        public int Count { get; set; }
    }
}
