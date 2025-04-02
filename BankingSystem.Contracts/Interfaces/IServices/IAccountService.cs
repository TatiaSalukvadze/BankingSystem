using BankingSystem.Contracts.DTOs.OnlineBank;
using BankingSystem.Contracts.DTOs.UserBanking;
using BankingSystem.Contracts.Response;
using BankingSystem.Domain.Entities;

namespace BankingSystem.Contracts.Interfaces.IServices
{
    public interface IAccountService
    {
        Task<Response<Account>> CreateAccountAsync(CreateAccountDTO createAccountDto);
        Task<Response<PagingResponseDTO<SeeAccountsDTO>>> SeeAccountsAsync(string email, int page, int perPage);
        Task<SimpleResponse> DeleteAccountAsync(string IBAN);
        Task<Response<TransferAccountsDTO>> ValidateAccountsForOnlineTransferAsync(string fromIBAN,
            string toIBAN, string email, bool isSelfTransfer);
        Task<SimpleResponse> UpdateAccountsAmountAsync(int fromAccountId, int toAccountId,
            decimal amountFromAccount, decimal amountToAccount);
        Task<SimpleResponse> UpdateBalanceForATMAsync(int accountId, decimal amountToDeduct);
    }
}
