using BankingSystem.Domain.Validation;
using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Domain.Entities
{
    public class TransactionDetails
    {
        public int Id { get; set; }

        [Required]
        [GreaterThanOrEqualToZero(ErrorMessage = "ბანკის მოგება უნდა იყოს 0-ზე მეტი.")] //მოგებააა????????
        public decimal BankProfit { get; set; }

        [Required]
        [GreaterThanOrEqualToZero(ErrorMessage = "თანხა უნდა იყოს 0 ან 0-ზე მეტი.")]
        public decimal Amount { get; set; }

        [Required]
        public int FromAccountId { get; set; }

        [Required]
        public int ToAccountId { get; set; }

        public bool IsATM { get; set; }


        public DateTime PerformedAt { get; set; }
    }
}
