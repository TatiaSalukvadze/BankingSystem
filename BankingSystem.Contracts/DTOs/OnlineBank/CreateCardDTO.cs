﻿using BankingSystem.Contracts.Validation;
using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Contracts.DTOs.OnlineBank
{
    public class CreateCardDTO
    {
        [Required, StringLength(22, MinimumLength = 22)]
        [RegularExpression("^GE[0-9]{2}CD[0-9]{16}$", ErrorMessage = "არასწორი IBAN ფორმატი.")]
        public string IBAN { get; set; }

        [Required, StringLength(16, MinimumLength = 16)]
        [RegularExpression("^[0-9]{16}$", ErrorMessage = "ბარათის ნომერი უნდა იყოს 16 ციფრისგან შემდგარი.")]
        public string CardNumber { get; set; }

        [Required]
        [RegularExpression("^(0[1-9]|1[0-2])/[0-9]{2}$", ErrorMessage = "ვადის ამოწურვის თარიღი უნდა იყოს ფორმატში MM/YY")]
        [ExpirationDate(ErrorMessage = "ვადის ამოწურვის თარიღი უნდა იყოს მომავალში.")]
        public string ExpirationDate { get; set; }

        [Required, StringLength(3)]
        [RegularExpression("^[0-9]{3}$", ErrorMessage = "CVV კოდი უნდა იყოს 3 ციფრისგან შემდგარი.")]
        public string CVV { get; set; }

        [Required, StringLength(4)]
        [RegularExpression("^[0-9]{4}$", ErrorMessage = "PIN კოდი უნდა იყოს 4 ციფრისგან შემდგარი.")]
        public string PIN { get; set; }
    }
}
