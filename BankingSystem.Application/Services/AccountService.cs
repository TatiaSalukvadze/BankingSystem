using BankingSystem.Contracts.DTOs.OnlineBank;
using BankingSystem.Contracts.DTOs.UserBanking;
using BankingSystem.Contracts.Interfaces;
using BankingSystem.Contracts.Interfaces.IServices;
using BankingSystem.Domain.Entities;
using BankingSystem.Domain.Enums;
namespace BankingSystem.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AccountService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        //tatia
        public async Task<(bool Success, string Message, Account? Data)> CreateAccountAsync(CreateAccountDTO createAccountDto)
        {

            int personId = await _unitOfWork.PersonRepository.FindIdByIDNumberAsync(createAccountDto.IDNumber);
            if (personId <= 0)
            {
                return (false, "Such person doesn't exist in our system!", null);
            }

            bool IBANExists = await _unitOfWork.AccountRepository.IBANExists(createAccountDto.IDNumber);
            if (IBANExists)
            {
                return (false, "Such IBAN already exist in our system!", null);
            }
            //int currencyId = await _unitOfWork.CurrencyRepository.FindIdByTypeAsync(createAccountDto.Currency.ToString());
            //if (currencyId <= 0)
            //{
            //    return (false, "Such Currency does not exist in our system!", null);
            //}
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
                return (false, "Account could not be created, something happened!", null);
            }
            account.Id = insertedId;

            _unitOfWork.SaveChanges();
            return (true, "Account was created successfully!", account);
        }

        //tamar
        public async Task<(bool success, string message, List<SeeAccountsDTO>? data)> SeeAccountsAsync(string email)
        {
            bool accountsExist = await _unitOfWork.AccountRepository.AccountExistForEmail(email);

            if (!accountsExist)
            {
                return (false, "You don't have any accounts!", null);
            }

            var accounts = await _unitOfWork.AccountRepository.SeeAccountsByEmail(email);

            if (accounts == null || accounts.Count == 0)
            {
                return (false, "No accounts found!", null);
            }

            return (true, "Accounts retrieved successfully!", accounts);
        }

        //tamar
        public async Task<(bool success, string message)> DeleteAccountAsync(string iban)
        {
            bool exists = await _unitOfWork.AccountRepository.IBANExists(iban);
            if (!exists)
            {
                return (false, "Account not found.");
            }

            var balance = await _unitOfWork.AccountRepository.GetBalanceByIBANAsync(iban);
            if (balance > 0)
            {
                return (false, "Account cannot be deleted while it has a balance.");
            }

            bool deleted = await _unitOfWork.AccountRepository.DeleteAccountByIBANAsync(iban);
            if (!deleted)
            {
                return (false, "Failed to delete account.");
            }

            _unitOfWork.SaveChanges();
            return (true, "Account deleted successfully.");
        }
        //for transaction changes
        //tatia
        public async Task<(bool Validated, string Message, Account from, Account to)> ValidateAccountsForOnlineTransferAsync(string fromIBAN,
            string toIBAN, string email, bool isSelfTransfer)
        {
            if (fromIBAN == toIBAN)
            {
                return (false, "You can't transfer to same account!", null, null);
            }
            var fromAccount = await _unitOfWork.AccountRepository.FindAccountByIBANandEmailAsync(fromIBAN, email);
            Account toAccount;
            if (isSelfTransfer)
            {
                toAccount = await _unitOfWork.AccountRepository.FindAccountByIBANandEmailAsync(toIBAN, email);
            }
            else
            {
                toAccount = await _unitOfWork.AccountRepository.FindAccountByIBANAsync(toIBAN);
            }
            if (fromAccount is null || toAccount is null)
            {
                return (false, "There is no account for one or both provided IBANs, check well!", null, null);
            }

            //if (fromAccount.Amount < amountToTransfer)
            //{
            //    return (false, "You don't have enough money to transfer on your account!", null, null);
            //}
            return (true, "Accounts validated!", fromAccount, toAccount);
        }
        //tatia
        public async Task<bool> UpdateAccountsAmountAsync(int fromAccountId, int toAccountId,
            decimal amountFromAccount, decimal amountToAccount)
        {
            var fromAccountUpdated = await _unitOfWork.AccountRepository.UpdateAccountAmountAsync(fromAccountId, -amountFromAccount);
            var toAccountUpdated = await _unitOfWork.AccountRepository.UpdateAccountAmountAsync(toAccountId, amountToAccount);

            if (!fromAccountUpdated || !toAccountUpdated)
            {
                return false;
            }
            return true;
        }

        //for atm --- feec unda gvaitvaliswino
        //tamar 
        public async Task<(bool success, string message, decimal balance, string currency)> CheckBalanceAndWithdrawalLimitAsync(string cardNumber, string pin, decimal withdrawalAmount)
        {
            var result = await _unitOfWork.AccountRepository.GetBalanceAndWithdrawnAmountAsync(cardNumber, pin);

            if (result == null)
            {
                return (false, "Unable to retrieve account details.", 0, null);
            }

            decimal balance = result.Amount;
            decimal totalWithdrawnIn24Hours = result.WithdrawnAmountIn24Hours;
            string currency = result.Currency;

            decimal newTotal = totalWithdrawnIn24Hours + withdrawalAmount;

            if (newTotal > 10000)
            {
                return (false, "You can't withdraw more than 10,000 within 24 hours.", balance,currency);
            }

            return (true, "", balance,  currency);
        }

        public async Task<(bool success, string message)> UpdateBalanceAsync(int accountId, decimal amountToDeduct)
        {
            bool isBalanceUpdated = await _unitOfWork.AccountRepository.UpdateAccountBalanceAsync(accountId, amountToDeduct);

            if (!isBalanceUpdated)
            {
                return (false, "Failed to update account balance.");
            }

            return (true, "Balance updated successfully.");
        }

    }
}
