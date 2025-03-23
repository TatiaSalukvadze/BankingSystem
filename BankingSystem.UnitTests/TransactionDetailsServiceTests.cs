using BankingSystem.Application.Services;
using BankingSystem.Contracts.DTOs.ATM;
using BankingSystem.Contracts.DTOs.Report;
using BankingSystem.Contracts.Interfaces;
using BankingSystem.Contracts.Interfaces.IExternalServices;
using BankingSystem.Contracts.Interfaces.IRepositories;
using BankingSystem.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Moq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BankingSystem.UnitTests
{
    public class TransactionDetailsServiceTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IExchangeRateService> _mockExchangeRateService;
        private readonly TransactionDetailsService _transactionDetailsService;

        public TransactionDetailsServiceTests()
        {
            var cardRepositoryMock = new Mock<ICardRepository>();
            var transactionDetailsRepositoryMock = new Mock<ITransactionDetailsRepository>();

            _mockConfiguration = new Mock<IConfiguration>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockExchangeRateService = new Mock<IExchangeRateService>();

            _mockUnitOfWork.Setup(u => u.CardRepository).Returns(cardRepositoryMock.Object);
            _mockUnitOfWork.Setup(u => u.TransactionDetailsRepository).Returns(transactionDetailsRepositoryMock.Object);

            _transactionDetailsService = new TransactionDetailsService(_mockConfiguration.Object, _mockUnitOfWork.Object, _mockExchangeRateService.Object);
        }

        [Fact]
        public async Task NumberOfTransactionsAsync_ShouldReturnNumberOfTransactions()
        {
            var transactionCount = new TransactionCountDTO { };
            _mockUnitOfWork.Setup(u => u.TransactionDetailsRepository.NumberOfTransactionsAsync()).ReturnsAsync(transactionCount);

            var response = await _transactionDetailsService.NumberOfTransactionsAsync();

            Assert.True(response.Success);
            Assert.Equal("Transaction Count retreived!", response.Message);
            Assert.NotNull(response.Data);
            Assert.Equal(200, response.StatusCode);

            _mockUnitOfWork.Verify(u => u.TransactionDetailsRepository, Times.Once);
        }

        [Fact]
        public async Task NumberOfTransactionsAsync_ShouldNotReturnNumberOfTransactions()
        {
            TransactionCountDTO transactionCount = null;
            _mockUnitOfWork.Setup(u => u.TransactionDetailsRepository.NumberOfTransactionsAsync()).ReturnsAsync(transactionCount);

            var response = await _transactionDetailsService.NumberOfTransactionsAsync();

            Assert.False(response.Success);
            Assert.Equal("Transaction Count couldn't be retreived!", response.Message);
            Assert.Null(response.Data);
            Assert.Equal(404, response.StatusCode);

            _mockUnitOfWork.Verify(u => u.TransactionDetailsRepository, Times.Once);
        }

        [Fact]
        public async Task AverageBankProfitAsync_ShouldReturnAverageBankProfit()
        {
            var expectedData = new Dictionary<string, decimal>{ { "USD", 1500} };

            _mockUnitOfWork.Setup(u => u.TransactionDetailsRepository.AverageBankProfitAsync()).ReturnsAsync(expectedData);

            var response = await _transactionDetailsService.AverageBankProfitAsync();

            Assert.True(response.Success);
            Assert.Equal("Bank profit count retrieved!", response.Message);
            Assert.NotNull(response.Data);
            Assert.Equal(expectedData, response.Data);
            Assert.Equal(200, response.StatusCode);

            _mockUnitOfWork.Verify(u => u.TransactionDetailsRepository.AverageBankProfitAsync(), Times.Once);
        }

        [Fact]
        public async Task AverageBankProfitAsync_ShouldNotReturnAverageBankProfit()
        {
            var expectedData = new Dictionary<string, decimal> { };

            _mockUnitOfWork.Setup(u => u.TransactionDetailsRepository.AverageBankProfitAsync()).ReturnsAsync(expectedData);

            var response = await _transactionDetailsService.AverageBankProfitAsync();

            Assert.False(response.Success);
            Assert.Equal("Bank profit couldn't be retrieved!", response.Message);
            Assert.Null(response.Data);
            Assert.NotEqual(expectedData, response.Data);
            Assert.Equal(404, response.StatusCode);

            _mockUnitOfWork.Verify(u => u.TransactionDetailsRepository.AverageBankProfitAsync(), Times.Once);
        }


        [Fact]
        public async Task TotalAtmWithdrawalsAsync_ShouldReturnTotalAtmWithdrawals()
        {
            var expectedData = new List<TotalAtmWithdrawalDTO> { };
            
            _mockUnitOfWork.Setup(u => u.TransactionDetailsRepository.GetTotalAtmWithdrawalsAsync()).ReturnsAsync(expectedData);

            var response = await _transactionDetailsService.TotalAtmWithdrawalsAsync();

            Assert.True(response.Success);
            Assert.Equal("ATM withdrawals data retrieved successfully.", response.Message);
            Assert.NotNull(response.Data);
            Assert.Equal(expectedData, response.Data);
            Assert.Equal(200, response.StatusCode);

            _mockUnitOfWork.Verify(u => u.TransactionDetailsRepository.GetTotalAtmWithdrawalsAsync(), Times.Once);
        }

        [Fact]
        public async Task TotalAtmWithdrawalsAsync_ShouldNotReturnTotalAtmWithdrawals()
        {
            List<TotalAtmWithdrawalDTO> expectedData = null;

            _mockUnitOfWork.Setup(u => u.TransactionDetailsRepository.GetTotalAtmWithdrawalsAsync()).ReturnsAsync(expectedData);

            var response = await _transactionDetailsService.TotalAtmWithdrawalsAsync();

            Assert.False(response.Success);
            Assert.Equal("No ATM withdrawal data found.", response.Message);
            Assert.Null(response.Data);
            Assert.Equal(404, response.StatusCode);

            _mockUnitOfWork.Verify(u => u.TransactionDetailsRepository.GetTotalAtmWithdrawalsAsync(), Times.Once);
        }

        [Fact]
        public async Task CalculateTransferAmountAsync_ShouldCalculateTransferAmounts()
        {
            string fromCurrency = "EUR";
            string toCurrency = "GEL";
            decimal amountToTransfer = 100;
            bool isSelfTransfer = true;
            decimal currencyRate = 3;
            decimal fee = 0;
            decimal extraFeeValue = 0;

            _mockExchangeRateService.Setup(x => x.GetCurrencyRateAsync(fromCurrency, toCurrency)).ReturnsAsync(currencyRate);
           
            var mockedSection = new Mock<IConfigurationSection>();
            mockedSection.Setup(x => x.Value).Returns("0");
            _mockConfiguration.Setup(x => x.GetSection("TransactionFees:SelfTransferPercent")).Returns(mockedSection.Object);

            var response = await _transactionDetailsService.CalculateTransferAmountAsync(fromCurrency, toCurrency, amountToTransfer, isSelfTransfer);
            var transferAmounts = response.Data;
            decimal expectedBankProfit = amountToTransfer * fee / 100 + extraFeeValue;
            decimal expectedAmountFromAccount = amountToTransfer + expectedBankProfit;  
            decimal expectedAmountToAccount = amountToTransfer * currencyRate;

            Assert.True(response.Success);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(expectedBankProfit, transferAmounts.BankProfit);
            Assert.Equal(expectedAmountFromAccount, transferAmounts.AmountFromAccount);
            Assert.Equal(expectedAmountToAccount, transferAmounts.AmountToAccount);
        }

        [Fact]
        public async Task CalculateTransferAmountAsync_ShouldNotCalculateTransferAmounts()
        {
            string fromCurrency = "EUR";
            string toCurrency = "GEL";
            decimal amountToTransfer = 100;
            bool isSelfTransfer = true;
            decimal currencyRate = 0;

            _mockExchangeRateService.Setup(x => x.GetCurrencyRateAsync(fromCurrency, toCurrency)).ReturnsAsync(currencyRate);

            var response = await _transactionDetailsService.CalculateTransferAmountAsync(fromCurrency, toCurrency, amountToTransfer, isSelfTransfer);
         
            Assert.False(response.Success);
            Assert.Equal("One of the account has incorrect currency!", response.Message);
            Assert.Equal(400, response.StatusCode);
        }
    }
}
