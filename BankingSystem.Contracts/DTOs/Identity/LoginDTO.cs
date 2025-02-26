using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Contracts.DTOs.Auth
{
    public class LoginDTO
    {
        [Required]
        public string? Username { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
