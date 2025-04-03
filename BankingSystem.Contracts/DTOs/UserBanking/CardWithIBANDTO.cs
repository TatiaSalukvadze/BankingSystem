using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Contracts.DTOs.UserBanking
{
    public class CardWithIBANDTO
    {
        [Required, StringLength(22, MinimumLength = 22)]
        //[RegularExpression("^GE[0-9]{2}CD[0-9]{16}$", ErrorMessage = "არასწორი IBAN ფორმატი.")]
        public string IBAN { get; set; }

        [Required, StringLength(50, MinimumLength = 2)]
        //[RegularExpression(@"^[ა-ჰ]+$|^[A-Za-z]+$", ErrorMessage = "სახელი უნდა შეიცავდეს მხოლოდ ქართულ ან მხოლოდ ლათინურ ასოებს.")]
        public string Name { get; set; }

        [Required, StringLength(50, MinimumLength = 2)]
        //[RegularExpression(@"^[ა-ჰ]+$|^[A-Za-z]+$", ErrorMessage = "გვარი უნდა შეიცავდეს მხოლოდ ქართულ ან მხოლოდ ლათინურ ასოებს.")]
        public string Surname { get; set; }

        [Required]
        public string CardNumber { get; set; }

        [Required]
        //[RegularExpression("^(0[1-9]|1[0-2])/[0-9]{2}$", ErrorMessage = "ვადის ამოწურვის თარიღი უნდა იყოს ფორმატში MM/YY")]
        public string ExpirationDate { get; set; }

        [Required]
        public string CVV { get; set; }
    }
}
