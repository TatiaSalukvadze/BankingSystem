using BankingSystem.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace BankingSystem.Contracts.DTOs.ATM
{
    public class TotalAtmWithdrawalDTO
    {
        public decimal TotalWithdrawnAmount { get; set; }

        public CurrencyType Currency { get; set; }
    }
}
