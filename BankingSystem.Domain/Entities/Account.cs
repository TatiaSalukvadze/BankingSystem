using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Domain.Entities
{
    public class Account
    {
        public int Id { get; set; }

        [Required]
        public int PersonId { get; set; }

        [Required, StringLength(22, MinimumLength = 22)]
        [RegularExpression("^GE[0-9]{2}CD[0-9]{14}$", ErrorMessage = "არასწორი IBAN ფორმატი.")]
        public string IBAN { get; set; }

        [Required, Range(0, double.MaxValue)]
        //[RegularExpression(@"^\d{1,3}(,\d{3})*(\.\d{2})?$", ErrorMessage = "თანხის ფორმატი არასწორია (მაგ: 1,234.56 ან 1234.56).")]
        public decimal Amount { get; set; }

        [Required]
        public int CurrencyId { get; set; }
    }

}
