using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Contracts.DTOs.OnlineBank
{
    public class DeleteCardDTO
    {
        [Required, StringLength(16, MinimumLength = 16)]
        [RegularExpression("^[0-9]{16}$", ErrorMessage = "ბარათის ნომერი უნდა იყოს 16 ციფრისგან შემდგარი.")]
        public string CardNumber { get; set; }
    }
}
