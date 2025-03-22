using BankingSystem.Application.Services;
using BankingSystem.Contracts.Interfaces.IRepositories;
using BankingSystem.Contracts.Interfaces;
using Moq;
using BankingSystem.Domain.Enums;
using BankingSystem.Contracts.DTOs.OnlineBank;
using BankingSystem.Domain.Entities;
using BankingSystem.Contracts.DTOs.UserBanking;

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

        [Fact]
        public async Task CreateAccountAsync_ShouldNotCreateExistingAccount()
        {
            var createAccountDto = new CreateAccountDTO
            {
                IDNumber = "33001059400",
                IBAN = "GE38CD9670132279404900",
                Amount = 24,
                Currency = CurrencyType.EUR
            };

            _mockUnitOfWork.Setup(u => u.PersonRepository.FindIdByIDNumberAsync(createAccountDto.IDNumber)).ReturnsAsync(1);
            _mockUnitOfWork.Setup(u => u.AccountRepository.IBANExists(createAccountDto.IBAN)).ReturnsAsync(true);
            _mockUnitOfWork.Setup(u => u.AccountRepository.CreateAccountAsync(It.IsAny<Account>())).ReturnsAsync(0);

            var response = await _accountService.CreateAccountAsync(createAccountDto);
            Assert.False(response.Success);
            Assert.Equal("Such IBAN already exist in our system!", response.Message);
            Assert.Null(response.Data);
            Assert.Equal(409, response.StatusCode);

            _mockUnitOfWork.Verify(u => u.AccountRepository, Times.Once);
            //_mockUnitOfWork.Verify(u => u.SaveChanges(), Times.Never);
        }

        [Fact]
        public async Task SeeAccountsAsync_ShouldSeeAccounts()
        {
            var email = "shamugiatamar22@gmail.com";
            var accountsList = new List<SeeAccountsDTO> { new SeeAccountsDTO { } };

            _mockUnitOfWork.Setup(u => u.AccountRepository.AccountExistForEmail(email)).ReturnsAsync(true);
            _mockUnitOfWork.Setup(u => u.AccountRepository.SeeAccountsByEmail(email)).ReturnsAsync(accountsList);

            var response = await _accountService.SeeAccountsAsync(email);
            Assert.True(response.Success);
            Assert.Equal("Accounts retrieved successfully!", response.Message);
            Assert.NotNull(response.Data);
            Assert.Equal(200, response.StatusCode);

            _mockUnitOfWork.Verify(u => u.AccountRepository, Times.Exactly(2));
        }

        [Fact]
        public async Task SeeAccountsAsync_ShouldNotSeeAccounts()
        {
            var email = "shamugiatamar22@gmail.com";
            var accountsList = new List<SeeAccountsDTO> { new SeeAccountsDTO { } };

            _mockUnitOfWork.Setup(u => u.AccountRepository.AccountExistForEmail(email)).ReturnsAsync(false);
            _mockUnitOfWork.Setup(u => u.AccountRepository.SeeAccountsByEmail(email)).ReturnsAsync(accountsList);

            var response = await _accountService.SeeAccountsAsync(email);
            Assert.False(response.Success);
            Assert.Equal("You don't have any accounts!", response.Message);
            Assert.Null(response.Data);
            Assert.Equal(400, response.StatusCode);

            _mockUnitOfWork.Verify(u => u.AccountRepository, Times.Once);
        }

        [Fact]
        public async Task DeleteAccountAsync_ShouldDeleteAccount()
        {
            var iban = "GE38CD9670132279404900";

            _mockUnitOfWork.Setup(u => u.AccountRepository.IBANExists(iban)).ReturnsAsync(true); ;
            _mockUnitOfWork.Setup(u => u.AccountRepository.GetBalanceByIBANAsync(iban)).ReturnsAsync(0);
            _mockUnitOfWork.Setup(u => u.AccountRepository.DeleteAccountByIBANAsync(iban)).ReturnsAsync(true);

            var response = await _accountService.DeleteAccountAsync(iban);
            Assert.True(response.Success);
            Assert.Equal("Account deleted successfully.", response.Message);
            Assert.Equal(200, response.StatusCode);

            _mockUnitOfWork.Verify(u => u.AccountRepository, Times.Exactly(3));
            //_mockUnitOfWork.Verify(u => u.SaveChanges(), Times.Once);
        }

        [Fact]
        public async Task DeleteAccountAsync_ShouldNotDeleteAccount()
        {
            var iban = "GE38CD9670132279404900";

            _mockUnitOfWork.Setup(u => u.AccountRepository.IBANExists(iban)).ReturnsAsync(true); ;
            _mockUnitOfWork.Setup(u => u.AccountRepository.GetBalanceByIBANAsync(iban)).ReturnsAsync(1);
            _mockUnitOfWork.Setup(u => u.AccountRepository.DeleteAccountByIBANAsync(iban)).ReturnsAsync(true);

            var response = await _accountService.DeleteAccountAsync(iban);
            Assert.False(response.Success);
            Assert.Equal("Account cannot be deleted while it has a balance.", response.Message);
            Assert.Equal(400, response.StatusCode);

            _mockUnitOfWork.Verify(u => u.AccountRepository, Times.Exactly(2));
        }

        [Fact]
        public async Task ValidateAccountsForOnlineTransferAsync_ShouldValidateAccountsForOnlineTransfer()
        {
            var fromIBAN = "GE38CD9670132279404900";
            var toIBAN = "GE38CD9670132279404910";
            var email = "shamugiatamar22@gmail.com";
            bool isSelfTransfer = false;
            var account = new Account();

            _mockUnitOfWork.Setup(u => u.AccountRepository.FindAccountByIBANandEmailAsync(fromIBAN, email)).ReturnsAsync(account);
            //_mockUnitOfWork.Setup(u => u.AccountRepository.FindAccountByIBANandEmailAsync(toIBAN, email)).ReturnsAsync(account);
            _mockUnitOfWork.Setup(u => u.AccountRepository.FindAccountByIBANAsync(toIBAN)).ReturnsAsync(account);

            var response = await _accountService.ValidateAccountsForOnlineTransferAsync(fromIBAN, toIBAN, email, isSelfTransfer);
            Assert.True(response.Success);
            Assert.Equal("Accounts validated!", response.Message);
            Assert.NotNull(response.Data);
            Assert.Equal(200, response.StatusCode);

            _mockUnitOfWork.Verify(u => u.AccountRepository, Times.Exactly(2));
        }

        [Fact]
        public async Task ValidateAccountsForOnlineTransferAsync_ShouldNotValidateAccountsForOnlineTransfer()
        {
            var fromIBAN = "GE38CD9670132279404900";
            var toIBAN = "GE38CD9670132279404900";
            var email = "shamugiatamar22@gmail.com";
            bool isSelfTransfer = false;

            var response = await _accountService.ValidateAccountsForOnlineTransferAsync(fromIBAN, toIBAN, email, isSelfTransfer);

            Assert.False(response.Success);
            Assert.Equal("You can't transfer to same account!", response.Message);
            Assert.Null(response.Data);
            Assert.Equal(400, response.StatusCode);
        }

        [Fact]
        public async Task UpdateAccountsAmountAsync_ShouldUpdateAccountsAmount()
        {
            int fromAccountId = 1;
            int toAccountId = 2;
            decimal amountFromAccount = 100;
            decimal amountToAccount = 100;

            _mockUnitOfWork.Setup(u => u.AccountRepository.UpdateAccountAmountAsync(fromAccountId, -amountFromAccount)).ReturnsAsync(true);
            _mockUnitOfWork.Setup(u => u.AccountRepository.UpdateAccountAmountAsync(toAccountId, amountToAccount)).ReturnsAsync(true);

            var result = await _accountService.UpdateAccountsAmountAsync(fromAccountId, toAccountId, amountFromAccount, amountToAccount);
            Assert.True(result.Success);
            Assert.Equal("Balance updated successfully!", result.Message);
            Assert.Equal(200, result.StatusCode);

            _mockUnitOfWork.Verify(u => u.AccountRepository, Times.Exactly(2));
        }

        [Fact]
        public async Task UpdateAccountsAmountAsync_ShouldNotUpdateAccountsAmount()
        {
            int fromAccountId = 1;
            int toAccountId = 2;
            decimal amountFromAccount = 100;
            decimal amountToAccount = 100;

            _mockUnitOfWork.Setup(u => u.AccountRepository.UpdateAccountAmountAsync(fromAccountId, -amountFromAccount)).ReturnsAsync(true);
            _mockUnitOfWork.Setup(u => u.AccountRepository.UpdateAccountAmountAsync(toAccountId, amountToAccount)).ReturnsAsync(false);

            var result = await _accountService.UpdateAccountsAmountAsync(fromAccountId, toAccountId, amountFromAccount, amountToAccount);
            Assert.False(result.Success);
            Assert.Equal("Balance couldn't be updated!", result.Message);
            Assert.Equal(400, result.StatusCode);

            _mockUnitOfWork.Verify(u => u.AccountRepository, Times.Exactly(2));
        }

        [Fact]
        public async Task UpdateBalanceForATMAsync_ShouldUpdateBalance()
        {
            int accountId = 1;
            decimal amountToDeduct = 50;

            _mockUnitOfWork.Setup(u => u.AccountRepository.UpdateAccountAmountAsync(accountId, amountToDeduct)).ReturnsAsync(true);

            var response = await _accountService.UpdateBalanceForATMAsync(accountId, amountToDeduct);
            Assert.True(response.Success);
            Assert.Equal("Balance updated successfully.", response.Message);
            Assert.Equal(200, response.StatusCode);

            _mockUnitOfWork.Verify(u => u.AccountRepository, Times.Once);
        }

        [Fact]
        public async Task UpdateBalanceForATMAsync_ShouldNotUpdateBalance()
        {
            int accountId = 1;
            decimal amountToDeduct = 50;

            _mockUnitOfWork.Setup(u => u.AccountRepository.UpdateAccountAmountAsync(accountId, amountToDeduct)).ReturnsAsync(false);

            var response = await _accountService.UpdateBalanceForATMAsync(accountId, amountToDeduct);
            Assert.False(response.Success);
            Assert.Equal("Failed to update account balance.", response.Message);
            Assert.Equal(400, response.StatusCode);

            _mockUnitOfWork.Verify(u => u.AccountRepository, Times.Once);
        }
    }
}
