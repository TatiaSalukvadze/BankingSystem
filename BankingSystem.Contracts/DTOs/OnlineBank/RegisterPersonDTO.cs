using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Contracts.DTOs.OnlineBank
{
    public class RegisterPersonDTO
    {
        [Required, StringLength(50, MinimumLength = 2)]
        [RegularExpression(@"^[ა-ჰ]+$|^[A-Za-z]+$", ErrorMessage = "სახელი უნდა შეიცავდეს მხოლოდ ქართულ ან მხოლოდ ლათინურ ასოებს.")]
        public string Name { get; set; }

        [Required, StringLength(50, MinimumLength = 2)]
        [RegularExpression(@"^[ა-ჰ]+$|^[A-Za-z]+$", ErrorMessage = "გვარი უნდა შეიცავდეს მხოლოდ ქართულ ან მხოლოდ ლათინურ ასოებს.")]
        public string Surname { get; set; }

        [Required, StringLength(11, MinimumLength = 11)]
        [RegularExpression("^[0-9]{11}$", ErrorMessage = "პირადი ნომერი უნდა შეიცავდეს 11 ციფრს.")]
        public string IDNumber { get; set; }

        [Required, DataType(DataType.Date)]
        public DateTime Birthdate { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(8, ErrorMessage = "პაროლი უნდა შეიცავდეს მინიმუმ 8 სიმბოლოს.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&.])[A-Za-z\d@$!%*?&.]{8,}$",
            ErrorMessage = "პაროლი უნდა შეიცავდეს მინიმუმ ერთ ციფრს, ერთ დიდ და პატარა ასოს და ერთ სიმბოლოს.")]
        public string Password { get; set; }

        public string ClientUrl { get; set; }
    }
}
