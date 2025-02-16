using BankingSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BankingSystem.Contracts.DTOs
{
    public class CreateAccountDTO
    {
        [Required, StringLength(11, MinimumLength = 11)]
        [RegularExpression("^[0-9]{11}$", ErrorMessage = "პირადი ნომერი უნდა შეიცავდეს 11 ციფრს!")]
        public string IDNumber { get; set; }
        [Required, StringLength(22, MinimumLength = 22)]
        [RegularExpression("^GE[0-9]{2}CD[0-9]{16}$", ErrorMessage = "არასწორი IBAN ფორმატი.")]
        public string IBAN { get; set; }

        [Required, Range(0, double.MaxValue)]
        //[RegularExpression(@"^\d{1,3}(,\d{3})*(\.\d{2})?$", ErrorMessage = "თანხის ფორმატი არასწორია (მაგ: 1,234.56 ან 1234.56).")]
        public decimal Amount { get; set; } = 0;
        [Required]
        public CurrenctType Currency { get; set; }
    }
}
