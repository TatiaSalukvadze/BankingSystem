using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Contracts.DTOs.Auth
{
    public class ResetPasswordDTO
    {
        [Required]
        [MinLength(8, ErrorMessage = "პაროლი უნდა შეიცავდეს მინიმუმ 8 სიმბოლოს.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&.])[A-Za-z\d@$!%*?&.]{8,}$",
            ErrorMessage = "პაროლი უნდა შეიცავდეს მინიმუმ ერთ ციფრს, ერთ დიდ და პატარა ასოს და ერთ სიმბოლოს.")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "პაროლები არ ემთხვევა ერთმანეთს!")]
        public string ConfirmPassword { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Token { get; set; }
    }
}
