using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Contracts.DTOs.OnlineBank
{
    public class RegisterPersonDTO
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Surname { get; set; }

        [Required]
        public string IDNumber { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime Birthdate { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string ClientUrl { get; set; }
    }
}
