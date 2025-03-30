using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Contracts.DTOs.Auth
{
    public class EmailConfirmationDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Token { get; set; }
    }
}
