using BankingSystem.Domain.Entities;

namespace BankingSystem.Contracts.DTOs.UserBanking
{
    public class TransferAccountsDTO
    {
        public Account From { get; set; }
        public Account To { get; set; }
    }
}
