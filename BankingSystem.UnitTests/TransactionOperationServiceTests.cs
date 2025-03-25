using BankingSystem.Application.FacadeServices;
using BankingSystem.Application.Services;
using BankingSystem.Contracts.DTOs.ATM;
using BankingSystem.Contracts.DTOs.UserBanking;
using BankingSystem.Contracts.Interfaces.IServices;
using BankingSystem.Contracts.Response;
using BankingSystem.Domain.Entities;
using BankingSystem.Domain.Enums;
using Moq;
using System.Net.NetworkInformation;

namespace BankingSystem.UnitTests
{
    public class TransactionOperationServiceTests
    {
        private readonly Mock<IAccountService> _mockAccountService;
        private readonly Mock<ICardService> _mockCardService;
        private readonly Mock<ITransactionDetailsService> _mockTransactionDetailsService;
        private readonly TransactionOperationService _transactionOperationService;

        public TransactionOperationServiceTests()
        {
            _mockAccountService = new Mock<IAccountService>();
            _mockCardService = new Mock<ICardService>();
            _mockTransactionDetailsService = new Mock<ITransactionDetailsService>();

            _transactionOperationService = new TransactionOperationService(_mockAccountService.Object, _mockCardService.Object, _mockTransactionDetailsService.Object);
        }

        [Fact]
        public async Task OnlineTransactionAsync_ShouldTransferFromAccountToAccount()
        {
            var createTransactionDto = new CreateOnlineTransactionDTO()
            {
                Amount = 20,
                FromIBAN = "GE38CD9670132279404959",
                ToIBAN = "GE38CD9670132279404958"
            };
            string email = "t@gmail.com";
            bool isSelfTransfer = false;

            var transferAccounts = new TransferAccountsDTO() { From = new Account() { Amount = 500}, To = new Account() };
            var validateAccountsResponse = new Response<TransferAccountsDTO>().Set(true, "Accounts validated!", transferAccounts, 200);
            _mockAccountService.Setup(a => a.ValidateAccountsForOnlineTransferAsync(createTransactionDto.FromIBAN,
                createTransactionDto.ToIBAN, email, isSelfTransfer)).ReturnsAsync(validateAccountsResponse);

            var transferAmounts = new TransferAmountCalculationDTO() { AmountFromAccount = 22, AmountToAccount = 20, BankProfit = 2 };
            var calculateTransferAmountResponse = new Response<TransferAmountCalculationDTO>().Set(true, "", transferAmounts, 200);
            _mockTransactionDetailsService.Setup(td => td.CalculateTransferAmountAsync(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<decimal>(), It.IsAny<bool>())).ReturnsAsync(calculateTransferAmountResponse);

            var updateAccountsResponse = new SimpleResponse().Set(true, "Balance updated successfully!", 200);
            _mockAccountService.Setup(a => a.UpdateAccountsAmountAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<decimal>(), It.IsAny<decimal>()))
            .ReturnsAsync(updateAccountsResponse);

            var createTransactionResponse = new SimpleResponse().Set(true, "Transaction was successfull!", 200);
            _mockTransactionDetailsService.Setup(td => td.CreateTransactionAsync(It.IsAny<decimal>(), It.IsAny<decimal>(), It.IsAny<int>(), 
                It.IsAny<int>(), It.IsAny<string>(), false)).ReturnsAsync(createTransactionResponse);

            var response = await _transactionOperationService.OnlineTransactionAsync(createTransactionDto, email, isSelfTransfer);

            Assert.True(response.Success);
            Assert.Equal("Transaction was successfull!", response.Message);
            Assert.Equal(200, response.StatusCode);

            _mockAccountService.VerifyAll();
        }

        [Fact]
        public async Task OnlineTransactionAsync_ShouldNotTransferNotEnoughMoney()
        {
            var createTransactionDto = new CreateOnlineTransactionDTO()
            {
                Amount = 20,
                FromIBAN = "GE38CD9670132279404959",
                ToIBAN = "GE38CD9670132279404958"
            };
            string email = "t@gmail.com";
            bool isSelfTransfer = false;

            var transferAccounts = new TransferAccountsDTO() { From = new Account() { Amount = 20.5m }, To = new Account() };
            var validateAccountsResponse = new Response<TransferAccountsDTO>().Set(true, "Accounts validated!", transferAccounts, 200);
            _mockAccountService.Setup(a => a.ValidateAccountsForOnlineTransferAsync(createTransactionDto.FromIBAN,
                createTransactionDto.ToIBAN, email, isSelfTransfer)).ReturnsAsync(validateAccountsResponse);

            var transferAmounts = new TransferAmountCalculationDTO() { AmountFromAccount = 22, AmountToAccount = 20, BankProfit = 2 };
            var calculateTransferAmountResponse = new Response<TransferAmountCalculationDTO>().Set(true, "", transferAmounts, 200);
            _mockTransactionDetailsService.Setup(td => td.CalculateTransferAmountAsync(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<decimal>(), It.IsAny<bool>())).ReturnsAsync(calculateTransferAmountResponse);

            var response = await _transactionOperationService.OnlineTransactionAsync(createTransactionDto, email, isSelfTransfer);

            Assert.False(response.Success);
            Assert.Equal("You don't have enough money to transfer on your account!", response.Message);
            Assert.Equal(400, response.StatusCode);

            _mockAccountService.VerifyAll();
        }

        [Fact]
        public async Task WithdrawAsync_ShouldWithdrawMoneyFromAtm()
        {
            var withdrawalDto = new WithdrawalDTO { CardNumber = "1234567890123456", PIN = "7777", Amount = 444, Currency = CurrencyType.GEL };
            var card = new Card { AccountId = 1 };
            var withdrawalData = new AtmWithdrawalCalculationDTO { Fee = 1, Balance = 999, Currency = "USD", TotalAmountToDeduct = 100 };

            _mockCardService.Setup(x => x.AuthorizeCardAsync(withdrawalDto.CardNumber, withdrawalDto.PIN))
                .ReturnsAsync(new Response<Card> { Success = true, Message = "Card validated", Data = card, StatusCode = 200 });
            _mockTransactionDetailsService.Setup(x => x.CalculateATMWithdrawalTransactionAsync(withdrawalDto.CardNumber, withdrawalDto.PIN, withdrawalDto.Amount, withdrawalDto.Currency.ToString()))
                .ReturnsAsync(new Response<AtmWithdrawalCalculationDTO> { Success = true, Message = "", Data = withdrawalData, StatusCode = 200 });
            _mockAccountService.Setup(x => x.UpdateBalanceForATMAsync(card.AccountId, withdrawalData.TotalAmountToDeduct))
                .ReturnsAsync(new SimpleResponse { Success = true, Message = "Balance updated successfully.", StatusCode = 200 });
            _mockTransactionDetailsService.Setup(x => x.CreateTransactionAsync(withdrawalData.Fee, withdrawalData.Balance, card.AccountId, card.AccountId, withdrawalData.Currency, true))
                .ReturnsAsync(new SimpleResponse { Success = true, Message = "Transaction successful", StatusCode = 200 });

            var result = await _transactionOperationService.WithdrawAsync(withdrawalDto);

            Assert.True(result.Success);
            Assert.Equal("Transaction successful", result.Message);
            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public async Task WithdrawAsync_ShouldNotWithdrawMOneyFromAtm()
        {
            var withdrawalDto = new WithdrawalDTO { CardNumber = "1234567890123456", PIN = "7777", Amount = 444, Currency = CurrencyType.GEL };
            var card = new Card { AccountId = 1 };
            var withdrawalData = new AtmWithdrawalCalculationDTO { Fee = 1, Balance = 999, Currency = "USD", TotalAmountToDeduct = 100 };

            _mockCardService.Setup(x => x.AuthorizeCardAsync(withdrawalDto.CardNumber, withdrawalDto.PIN))
                .ReturnsAsync(new Response<Card> { Success = true, Message = "Card validated", Data = card, StatusCode = 200 });
            _mockTransactionDetailsService.Setup(x => x.CalculateATMWithdrawalTransactionAsync(withdrawalDto.CardNumber, withdrawalDto.PIN, withdrawalDto.Amount, withdrawalDto.Currency.ToString()))
                .ReturnsAsync(new Response<AtmWithdrawalCalculationDTO> { Success = true, Message = "", Data = withdrawalData, StatusCode = 200 });
            _mockAccountService.Setup(x => x.UpdateBalanceForATMAsync(card.AccountId, withdrawalData.TotalAmountToDeduct))
                .ReturnsAsync(new SimpleResponse { Success = false, Message = "Failed to update account balance.", StatusCode = 400 });

            var result = await _transactionOperationService.WithdrawAsync(withdrawalDto);

            Assert.False(result.Success);
            Assert.Equal("Failed to update account balance.", result.Message);
            Assert.Equal(400, result.StatusCode);
        }
    }
}
