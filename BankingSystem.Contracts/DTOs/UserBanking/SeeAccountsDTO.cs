using BankingSystem.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Contracts.DTOs.UserBanking
{
    public class SeeAccountsDTO
    {
        public string IBAN { get; set; }

        public decimal Amount { get; set; }

        public CurrencyType Currency { get; set; }
    }
}
