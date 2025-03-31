using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Domain.Entities
{
    public class Card
    {
        public int Id { get; set; }

        [Required]
        public int AccountId { get; set; }

        [Required]
        public string CardNumber { get; set; }

        [Required]
        [RegularExpression("^(0[1-9]|1[0-2])/[0-9]{2}$", ErrorMessage = "ვადის ამოწურვის თარიღი უნდა იყოს ფორმატში MM/YY")]
        public string ExpirationDate { get; set; }

        [Required]
        public string CVV { get; set; }

        [Required]
        public string PIN { get; set; }
    }
}
