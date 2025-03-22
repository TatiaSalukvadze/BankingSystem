using BankingSystem.Application.Services;
using BankingSystem.Contracts.Interfaces.IRepositories;
using BankingSystem.Contracts.Interfaces;
using Moq;
using BankingSystem.Domain.Enums;
using BankingSystem.Contracts.DTOs.OnlineBank;
using BankingSystem.Domain.Entities;
using NuGet.Frameworks;
using BankingSystem.Contracts.DTOs.UserBanking;
using Microsoft.AspNetCore.Routing;
using BankingSystem.Contracts.Interfaces.IExternalServices;
using Microsoft.Extensions.Configuration;
using BankingSystem.Contracts.DTOs.ATM;

namespace BankingSystem.UnitTests
{
    public class AccountServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly AccountService _accountService;

        public AccountServiceTests()
        {
            var accountRepositoryMock = new Mock<IAccountRepository>();
            var personRepositoryMock = new Mock<IPersonRepository>();

            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _mockUnitOfWork.Setup(u => u.PersonRepository).Returns(personRepositoryMock.Object);
            _mockUnitOfWork.Setup(u => u.AccountRepository).Returns(accountRepositoryMock.Object);

            _accountService = new AccountService(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task CreateAccountAsync_ShouldCreateAccount()
        {
            var createAccountDto = new CreateAccountDTO
            {
                IDNumber = "33001059400",
                IBAN = "GE38CD9670132279404900",
                Amount = 24,
                Currency = CurrencyType.EUR
            };

            _mockUnitOfWork.Setup(u => u.PersonRepository.FindIdByIDNumberAsync(createAccountDto.IDNumber)).ReturnsAsync(1);
            _mockUnitOfWork.Setup(u => u.AccountRepository.IBANExists(createAccountDto.IBAN)).ReturnsAsync(false);
            _mockUnitOfWork.Setup(u => u.AccountRepository.CreateAccountAsync(It.IsAny<Account>())).ReturnsAsync(1);

            var returnedResponse = await _accountService.CreateAccountAsync(createAccountDto);
            Assert.True(returnedResponse.Success);
            Assert.Equal("Account was created successfully!", returnedResponse.Message);
            Assert.NotNull(returnedResponse.Data);
            Assert.Equal(201, returnedResponse.StatusCode);

            _mockUnitOfWork.Verify(u => u.AccountRepository, Times.Exactly(2));
           // _mockUnitOfWork.Verify(u => u.SaveChanges(), Times.Once);
        }

        //[Fact]
        //public async Task CreateAccountAsync_ShouldNotCreateExistingAccount()
        //{
        //    var createAccountDto = new CreateAccountDTO { IDNumber = "33001059400", IBAN = "GE38CD9670132279404900",
        //    Amount = 24, Currency = CurrencyType.EUR };

        //    _mockUnitOfWork.Setup(u => u.PersonRepository.FindIdByIDNumberAsync(createAccountDto.IDNumber)).ReturnsAsync(1);
        //    _mockUnitOfWork.Setup(u => u.AccountRepository.IBANExists(createAccountDto.IBAN)).ReturnsAsync(true);
        //    _mockUnitOfWork.Setup(u => u.AccountRepository.CreateAccountAsync(It.IsAny<Account>())).ReturnsAsync(0);

        //    var (success, message, data) = await _accountService.CreateAccountAsync(createAccountDto);
        //    Assert.False(success);
        //    Assert.Equal("Such IBAN already exist in our system!", message);
        //    Assert.Null(data);

        //    _mockUnitOfWork.Verify(u => u.AccountRepository, Times.Once);
        //    _mockUnitOfWork.Verify(u => u.SaveChanges(), Times.Never);
        //}

        //[Fact]
        //public async Task SeeAccountsAsync_ShouldSeeAccounts()
        //{
        //    var email = "shamugiatamar22@gmail.com";
        //    var accountsList= new List<SeeAccountsDTO> { new SeeAccountsDTO { } };

        //    _mockUnitOfWork.Setup(u => u.AccountRepository.AccountExistForEmail(email)).ReturnsAsync(true);
        //    _mockUnitOfWork.Setup(u => u.AccountRepository.SeeAccountsByEmail(email)).ReturnsAsync(accountsList);

        //    var (success, message, data) = await _accountService.SeeAccountsAsync(email);
        //    Assert.True(success);
        //    Assert.Equal("Accounts retrieved successfully!", message);
        //    Assert.NotNull(data);

        //    _mockUnitOfWork.Verify(u => u.AccountRepository, Times.Exactly(2));
        //}

        //[Fact]
        //public async Task SeeAccountsAsync_ShouldNotSeeAccounts()
        //{
        //    var email = "shamugiatamar22@gmail.com";
        //    var accountsList = new List<SeeAccountsDTO> { new SeeAccountsDTO { } };

        //    _mockUnitOfWork.Setup(u => u.AccountRepository.AccountExistForEmail(email)).ReturnsAsync(false);
        //    _mockUnitOfWork.Setup(u => u.AccountRepository.SeeAccountsByEmail(email)).ReturnsAsync(accountsList);

        //    var (success, message, data) = await _accountService.SeeAccountsAsync(email);
        //    Assert.False(success);
        //    Assert.Equal("You don't have any accounts!", message);
        //    Assert.Null(data);

        //    _mockUnitOfWork.Verify(u => u.AccountRepository, Times.Once);
        //}

        //[Fact]
        //public async Task DeleteAccountAsync_ShouldDeleteAccount()
        //{
        //    var iban = "GE38CD9670132279404900";

        //    _mockUnitOfWork.Setup(u => u.AccountRepository.IBANExists(iban)).ReturnsAsync(true); ;
        //    _mockUnitOfWork.Setup(u => u.AccountRepository.GetBalanceByIBANAsync(iban)).ReturnsAsync(0);
        //    _mockUnitOfWork.Setup(u => u.AccountRepository.DeleteAccountByIBANAsync(iban)).ReturnsAsync(true);

        //    var (success, message) = await _accountService.DeleteAccountAsync(iban);
        //    Assert.True(success);
        //    Assert.Equal("Account deleted successfully.", message);

        //    _mockUnitOfWork.Verify(u => u.AccountRepository, Times.Exactly(3));
        //    _mockUnitOfWork.Verify(u => u.SaveChanges(), Times.Once);
        //}

        //[Fact]
        //public async Task DeleteAccountAsync_ShouldNotDeleteAccount()
        //{
        //    var iban = "GE38CD9670132279404900";

        //    _mockUnitOfWork.Setup(u => u.AccountRepository.IBANExists(iban)).ReturnsAsync(true); ;
        //    _mockUnitOfWork.Setup(u => u.AccountRepository.GetBalanceByIBANAsync(iban)).ReturnsAsync(1);
        //    _mockUnitOfWork.Setup(u => u.AccountRepository.DeleteAccountByIBANAsync(iban)).ReturnsAsync(true);

        //    var (success, message) = await _accountService.DeleteAccountAsync(iban);
        //    Assert.False(success);
        //    Assert.Equal("Account cannot be deleted while it has a balance.", message);

        //    _mockUnitOfWork.Verify(u => u.AccountRepository, Times.Exactly(2));
        //    _mockUnitOfWork.Verify(u => u.SaveChanges(), Times.Never);
        //}

        //[Fact]
        //public async Task ValidateAccountsForOnlineTransferAsync_ShouldValidateAccountsForOnlineTransfer()
        //{
        //    var fromIBAN = "GE38CD9670132279404900";
        //    var toIBAN = "GE38CD9670132279404910";
        //    var email = "shamugiatamar22@gmail.com";
        //    bool isSelfTransfer = false;
        //    var account = new Account();

        //    _mockUnitOfWork.Setup(u => u.AccountRepository.FindAccountByIBANandEmailAsync(fromIBAN, email)).ReturnsAsync(account);
        //    //_mockUnitOfWork.Setup(u => u.AccountRepository.FindAccountByIBANandEmailAsync(toIBAN, email)).ReturnsAsync(account);
        //    _mockUnitOfWork.Setup(u => u.AccountRepository.FindAccountByIBANAsync(toIBAN)).ReturnsAsync(account);

        //    var (validated, message, from, to) = await _accountService.ValidateAccountsForOnlineTransferAsync(fromIBAN, toIBAN, email, isSelfTransfer);
        //    Assert.True(validated);
        //    Assert.Equal("Accounts validated!", message);
        //    Assert.NotNull(from);
        //    Assert.NotNull(to);

        //    _mockUnitOfWork.Verify(u => u.AccountRepository, Times.Exactly(2));
        //}

        //[Fact]
        //public async Task ValidateAccountsForOnlineTransferAsync_ShouldNotValidateAccountsForOnlineTransfer()
        //{
        //    var fromIBAN = "GE38CD9670132279404900";
        //    var toIBAN = "GE38CD9670132279404900";
        //    var email = "shamugiatamar22@gmail.com";
        //    bool isSelfTransfer = false;

        //    var (validated, message, from, to) = await _accountService.ValidateAccountsForOnlineTransferAsync(fromIBAN, toIBAN, email, isSelfTransfer);

        //    Assert.False(validated);
        //    Assert.Equal("You can't transfer to same account!", message);
        //    Assert.Null(from);
        //    Assert.Null(to);
        //}

        //[Fact]
        //public async Task UpdateAccountsAmountAsync_ShouldUpdateAccountsAmount()
        //{
        //    int fromAccountId = 1;
        //    int toAccountId = 2;
        //    decimal amountFromAccount = 100;
        //    decimal amountToAccount = 100;

        //    _mockUnitOfWork.Setup(u => u.AccountRepository.UpdateAccountAmountAsync(fromAccountId, -amountFromAccount)).ReturnsAsync(true);
        //    _mockUnitOfWork.Setup(u => u.AccountRepository.UpdateAccountAmountAsync(toAccountId, amountToAccount)).ReturnsAsync(true); 

        //    var result = await _accountService.UpdateAccountsAmountAsync(fromAccountId, toAccountId, amountFromAccount, amountToAccount);
        //    Assert.True(result);
        //    _mockUnitOfWork.Verify(u => u.AccountRepository, Times.Exactly(2));
        //}

        //[Fact]
        //public async Task UpdateAccountsAmountAsync_ShouldNotUpdateAccountsAmount()
        //{
        //    int fromAccountId = 1;
        //    int toAccountId = 2;
        //    decimal amountFromAccount = 100;
        //    decimal amountToAccount = 100;

        //    _mockUnitOfWork.Setup(u => u.AccountRepository.UpdateAccountAmountAsync(fromAccountId, -amountFromAccount)).ReturnsAsync(true);
        //    _mockUnitOfWork.Setup(u => u.AccountRepository.UpdateAccountAmountAsync(toAccountId, amountToAccount)).ReturnsAsync(false);

        //    var result = await _accountService.UpdateAccountsAmountAsync(fromAccountId, toAccountId, amountFromAccount, amountToAccount);
        //    Assert.False(result);
        //    _mockUnitOfWork.Verify(u => u.AccountRepository, Times.Exactly(2));
        //}

        //[Fact]
        //public async Task UpdateBalanceForATMAsync_ShouldUpdateBalance()
        //{
        //    int accountId = 1;
        //    decimal amountToDeduct = 50;

        //    _mockUnitOfWork.Setup(u => u.AccountRepository.UpdateAccountBalanceAsync(accountId, amountToDeduct)).ReturnsAsync(true); 

        //    var (success, message) = await _accountService.UpdateBalanceForATMAsync(accountId, amountToDeduct);
        //    Assert.True(success);
        //    Assert.Equal("Balance updated successfully.", message);

        //    _mockUnitOfWork.Verify(u => u.AccountRepository, Times.Once);
        //}

        //[Fact]
        //public async Task UpdateBalanceForATMAsync_ShouldNotUpdateBalance()
        //{
        //    int accountId = 1;
        //    decimal amountToDeduct = 50;

        //    _mockUnitOfWork.Setup(u => u.AccountRepository.UpdateAccountBalanceAsync(accountId, amountToDeduct)).ReturnsAsync(false);

        //    var (success, message) = await _accountService.UpdateBalanceForATMAsync(accountId, amountToDeduct);
        //    Assert.False(success);
        //    Assert.Equal("Failed to update account balance.", message);

        //    _mockUnitOfWork.Verify(u => u.AccountRepository, Times.Once);
        //}
    }
}
