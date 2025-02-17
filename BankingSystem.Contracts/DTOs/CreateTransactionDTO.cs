using BankingSystem.Domain.Validation;
using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Contracts.DTOs
{
    public class CreateTransactionDTO
    {
        [Required, StringLength(22, MinimumLength = 22)]
        [RegularExpression("^GE[0-9]{2}CD[0-9]{16}$", ErrorMessage = "არასწორი IBAN ფორმატი.")]
        public string FromIBAN { get; set; }

        [Required, StringLength(22, MinimumLength = 22)]
        [RegularExpression("^GE[0-9]{2}CD[0-9]{16}$", ErrorMessage = "არასწორი IBAN ფორმატი.")]
        public string ToIBAN { get; set; }

        [Required]
        [GreaterThanOrEqualToZero(ErrorMessage = "თანხა უნდა იყოს 0 ან 0-ზე მეტი.")]
        public decimal Amount { get; set; }
    }
}
