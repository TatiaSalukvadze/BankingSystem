using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Contracts.DTOs.Identity
{
    public class ForgotPasswordDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string ClientUrl { get; set; }
    }
}
