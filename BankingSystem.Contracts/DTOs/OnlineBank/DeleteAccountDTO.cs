﻿using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Contracts.DTOs.OnlineBank
{
    public class DeleteAccountDTO
    {
        [Required, StringLength(22, MinimumLength = 22)]
        [RegularExpression("^GE[0-9]{2}CD[0-9]{16}$", ErrorMessage = "არასწორი IBAN ფორმატი.")]
        public string IBAN { get; set; }
    }
}
