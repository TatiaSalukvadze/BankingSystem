﻿using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Contracts.DTOs.ATM
{
    public class ChangeCardPINDTO 
    {
        [Required, StringLength(16, MinimumLength = 16)]
        [RegularExpression("^[0-9]{16}$", ErrorMessage = "ბარათის ნომერი უნდა იყოს 16 ციფრისგან შემდგარი.")]
        public string CardNumber { get; set; }

        [Required, StringLength(4)]
        [RegularExpression("^[0-9]{4}$", ErrorMessage = "ძველი PIN კოდი უნდა იყოს 4 ციფრისგან შემდგარი.")]
        public string PIN { get; set; }

        [Required, StringLength(4)]
        [RegularExpression("^[0-9]{4}$", ErrorMessage = "ახალი PIN კოდი უნდა იყოს 4 ციფრისგან შემდგარი.")]
        public string NewPIN { get; set; }
    }
}
