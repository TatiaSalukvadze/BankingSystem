using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Domain.Entities
{
    public class Person
    {
        public int Id { get; set; }

        [Required]
        public string IdentityUserId { get; set; }

        [Required, StringLength(50, MinimumLength = 2)]
        [RegularExpression(@"^[ა-ჰ]+$|^[A-Za-z]+$", ErrorMessage = "სახელი უნდა შეიცავდეს მხოლოდ ქართულ ან მხოლოდ ლათინურ ასოებს.")]
        //[RegularExpression(@"^([\p{IsGeorgian}]+|[a-zA-Z\s]+)$", ErrorMessage = "სახელი უნდა შეიცავდეს მხოლოდ ქართულ ან მხოლოდ ლათინურ ასოებს.")]
        public string Name { get; set; }

        [Required, StringLength(50, MinimumLength = 2)]
        [RegularExpression(@"^[ა-ჰ]+$|^[A-Za-z]+$", ErrorMessage = "სახელი უნდა შეიცავდეს მხოლოდ ქართულ ან მხოლოდ ლათინურ ასოებს.")]
        //[RegularExpression(@"^([\p{IsGeorgian}]+|[a-zA-Z\s]+)$", ErrorMessage = "გვარი უნდა შეიცავდეს მხოლოდ ქართულ ან მხოლოდ ლათინურ ასოებს.")]
        public string Surname { get; set; }

        [Required, StringLength(11, MinimumLength = 11)]
        [RegularExpression("^[0-9]{11}$", ErrorMessage = "პირადი ნომერი უნდა შეიცავდეს 11 ციფრს.")]
        public string IDNumber { get; set; }

        [Required, DataType(DataType.Date)]
        public DateTime Birthdate { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
