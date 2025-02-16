using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Domain.Entities
{
    public class CurrencyType
    {
        public int Id { get; set; }

        [Required]
        [RegularExpression("^[A-Z]{3}$", ErrorMessage = "ვალუტის კოდი უნდა შედგებოდეს 3 დიდი ასოსგან (მაგ: USD, GEL, EUR).")]
        public string Type { get; set; }
    }
}
