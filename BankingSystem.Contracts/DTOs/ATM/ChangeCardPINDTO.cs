using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Contracts.DTOs.ATM
{
    public class ChangeCardPINDTO
    {
        [Required, StringLength(16, MinimumLength = 16)]
        [RegularExpression("^[0-9]{16}$", ErrorMessage = "ბარათის ნომერი უნდა იყოს 16 ციფრისგან შემდგარი.")]
        public string CardNumber { get; set; }

        [Required, StringLength(4)]
        [RegularExpression("^[0-9]{4}$", ErrorMessage = "PIN კოდი უნდა იყოს 4 ციფრისგან შემდგარი.")]
        public string OldPIN { get; set; }

        [Required, StringLength(4)]
        [RegularExpression("^[0-9]{4}$", ErrorMessage = "PIN კოდი უნდა იყოს 4 ციფრისგან შემდგარი.")]
        public string NewPIN { get; set; }
    }
}
