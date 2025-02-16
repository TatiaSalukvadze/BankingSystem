﻿using BankingSystem.Domain.Enums;
using BankingSystem.Domain.Validation;
using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Contracts.DTOs
{
    public class CreateAccountDTO
    {
        [Required, StringLength(11, MinimumLength = 11)]
        [RegularExpression("^[0-9]{11}$", ErrorMessage = "პირადი ნომერი უნდა შეიცავდეს 11 ციფრს.")]
        public string IDNumber { get; set; }

        [Required, StringLength(22, MinimumLength = 22)]
        [RegularExpression("^GE[0-9]{2}CD[0-9]{16}$", ErrorMessage = "არასწორი IBAN ფორმატი.")]
        public string IBAN { get; set; }

        [Required]
        [GreaterThanZero(ErrorMessage = "თანხა უნდა იყოს 0-ზე მეტი.")]
        public decimal Amount { get; set; }
        [Required]
        public CurrenctType Currency { get; set; }
    }
}
