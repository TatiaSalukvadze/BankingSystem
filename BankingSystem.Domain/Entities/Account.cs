using BankingSystem.Domain.Validation;
using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Domain.Entities
{
    public class Account
    {
        public int Id { get; set; }

        [Required]
        public int PersonId { get; set; }

        [Required, StringLength(22, MinimumLength = 22)]
        [RegularExpression("^GE[0-9]{2}CD[0-9]{16}$", ErrorMessage = "არასწორი IBAN ფორმატი.")]
        public string IBAN { get; set; }

        [Required]
        [GreaterThanZero(ErrorMessage = "თანხა უნდა იყოს 0-ზე მეტი.")]
        public decimal Amount { get; set; }

        [Required]
        public int CurrencyId { get; set; }
    }

}
