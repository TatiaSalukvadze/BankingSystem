using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Domain.Entities
{
    public class TransactionDetails
    {
        public int Id { get; set; }

        [Required, Range(0, double.MaxValue)]
        public decimal BankProfit { get; set; }

        [Required, Range(0, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        public int FromAccountId { get; set; }

        [Required]
        public int ToAccountId { get; set; }

        [Required]
        public int CurrencyId { get; set; }

        [Required]
        public int BankingTypeId { get; set; }

        public DateTime PerformedAt { get; set; }
    }
}
