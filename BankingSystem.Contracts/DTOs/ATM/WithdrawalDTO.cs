using BankingSystem.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Contracts.DTOs.ATM
{
    public class WithdrawalDTO
    {
        [Required, StringLength(16, MinimumLength = 16)]
        [RegularExpression("^[0-9]{16}$", ErrorMessage = "ბარათის ნომერი უნდა იყოს 16 ციფრისგან შემდგარი.")]
        public string CardNumber { get; set; }

        [Required, StringLength(4)]
        [RegularExpression("^[0-9]{4}$", ErrorMessage = "PIN კოდი უნდა იყოს 4 ციფრისგან შემდგარი.")]
        public string PIN { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public CurrencyType Currency { get; set; }
    }
}
