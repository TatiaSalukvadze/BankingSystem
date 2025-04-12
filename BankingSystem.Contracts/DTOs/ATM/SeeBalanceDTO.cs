using BankingSystem.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Contracts.DTOs.ATM
{
    public class SeeBalanceDTO
    {
        public decimal Amount { get; set; }

        public CurrencyType Currency { get; set; }
    }
}
