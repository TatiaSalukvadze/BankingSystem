using BankingSystem.Contracts.DTOs.OnlineBank;
using BankingSystem.Contracts.DTOs.UserBanking;
using BankingSystem.Contracts.Interfaces;
using BankingSystem.Contracts.Interfaces.IServices;
using BankingSystem.Contracts.Response;
using BankingSystem.Domain.Entities;

namespace BankingSystem.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AccountService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Response<Account>> CreateAccountAsync(CreateAccountDTO createAccountDto)
        {
            var response = new Response<Account>();
            int personId = await _unitOfWork.PersonRepository.FindIdByIDNumberAsync(createAccountDto.IDNumber);
            if (personId <= 0)
            {
                return response.Set(false, "Such person doesn't exist in our system!", null, 404);
            }

            bool IBANExists = await _unitOfWork.AccountRepository.IBANExists(createAccountDto.IBAN);
            if (IBANExists)
            {
                return response.Set(false, "Such IBAN already exist in our system!", null, 409);
            }
            
            var account = new Account
            {
                PersonId = personId,
                IBAN = createAccountDto.IBAN,
                Amount = createAccountDto.Amount,
                Currency = createAccountDto.Currency.ToString()
            };

            int insertedId = await _unitOfWork.AccountRepository.CreateAccountAsync(account);
            if (insertedId <= 0)
            {
                return response.Set(false, "Account could not be created, something happened!", null, 400);
            }
            account.Id = insertedId;

            return response.Set(true, "Account was created successfully!", account, 201);
        }

        public async Task<Response<PagingResponseDTO<SeeAccountsDTO>>> SeeAccountsAsync(string email, int page, int perPage)
        {
            var response = new Response<PagingResponseDTO<SeeAccountsDTO>>();
            int accountsCount = await _unitOfWork.AccountRepository.AccountsCountForEmail(email);
            if (accountsCount <= 0)
            {
                return response.Set(false, "You don't have any accounts!", null, 400);
            }
            var offset = (page - 1) * perPage;
            var paginatedAccounts = await _unitOfWork.AccountRepository.SeeAccountsByEmail(email, offset, perPage);
            if (paginatedAccounts == null || paginatedAccounts.Count == 0)
            {
                return response.Set(false, "No accounts found!", null, 404);
            }
            int totalPages = (int)Math.Ceiling((double)accountsCount / perPage);
            var pagingResponse = new PagingResponseDTO<SeeAccountsDTO>()
            {
                Data = paginatedAccounts,
                TotalPages = totalPages,
                TotalDataCount = accountsCount,
                CurrentPage = page,
                DataCountPerPage = perPage,
                HasNext = page < totalPages,
                HasPrevious = page > 1

            };
            return response.Set(true, "Accounts retrieved successfully!", pagingResponse, 200);
        }

        public async Task<SimpleResponse> DeleteAccountAsync(string iban)
        {
            var response = new SimpleResponse();
            bool exists = await _unitOfWork.AccountRepository.IBANExists(iban);
            if (!exists)
            {
                return response.Set(false, "Account not found.", 404);
            }

            var balance = await _unitOfWork.AccountRepository.GetBalanceByIBANAsync(iban);
            if (balance > 0)
            {
                return response.Set(false, "Account cannot be deleted while it has a balance.", 400);
            }

            bool deleted = await _unitOfWork.AccountRepository.DeleteAccountByIBANAsync(iban);
            if (!deleted)
            {
                return response.Set(false, "Failed to delete account.", 400);
            }

            return response.Set(true, "Account deleted successfully.", 200);
        }

        #region transactionHelpers
        public async Task<Response<TransferAccountsDTO>> ValidateAccountsForOnlineTransferAsync(string fromIBAN,
            string toIBAN, string email, bool isSelfTransfer)
        {
            var response = new Response<TransferAccountsDTO>();
            if (fromIBAN == toIBAN)
            {
                return response.Set(false, "You can't transfer to same account!", null, 400);
            }

            var fromToAccounts = new TransferAccountsDTO();
            fromToAccounts.From = await _unitOfWork.AccountRepository.FindAccountByIBANandEmailAsync(fromIBAN, email);
            if (isSelfTransfer)
            {
                fromToAccounts.To = await _unitOfWork.AccountRepository.FindAccountByIBANandEmailAsync(toIBAN, email);
            }
            else
            {
                fromToAccounts.To = await _unitOfWork.AccountRepository.FindAccountByIBANAsync(toIBAN);
            }
            if (fromToAccounts.From is null || fromToAccounts.To is null)
            {
                return response.Set(false, "There is no account for one or both provided IBANs, check well!", null, 404);
            }

            return response.Set(true, "Accounts validated!", fromToAccounts, 200);
        }

        public async Task<SimpleResponse> UpdateAccountsAmountAsync(int fromAccountId, int toAccountId,
            decimal amountFromAccount, decimal amountToAccount)
        {
            var response = new SimpleResponse();
            _unitOfWork.BeginTransaction();
            _unitOfWork.AccountRepository.SetTransaction(_unitOfWork.Transaction());

            var fromAccountUpdated = await _unitOfWork.AccountRepository.UpdateAccountAmountAsync(fromAccountId, -amountFromAccount);
            var toAccountUpdated = await _unitOfWork.AccountRepository.UpdateAccountAmountAsync(toAccountId, amountToAccount);
            if (!fromAccountUpdated || !toAccountUpdated)
            {
                return response.Set(false, "Balance couldn't be updated!", 400);
            }
            return response.Set(true,"Balance updated successfully!", 200);
        }

        public async Task<SimpleResponse> UpdateBalanceForATMAsync(int accountId, decimal amountToDeduct)
        {
            var response = new SimpleResponse();
            _unitOfWork.BeginTransaction();
            _unitOfWork.AccountRepository.SetTransaction(_unitOfWork.Transaction());

            bool isBalanceUpdated = await _unitOfWork.AccountRepository.UpdateAccountAmountAsync(accountId, -amountToDeduct);
            if (!isBalanceUpdated)
            {
                return response.Set(false, "Failed to update account balance.", 400);
            }

            return response.Set(true, "Balance updated successfully.", 200);
        }
        #endregion
    }
}
