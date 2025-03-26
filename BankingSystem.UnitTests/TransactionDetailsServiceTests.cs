using BankingSystem.Application.Services;
using BankingSystem.Contracts.DTOs.ATM;
using BankingSystem.Contracts.DTOs.Report;
using BankingSystem.Contracts.DTOs.UserBanking;
using BankingSystem.Contracts.Interfaces;
using BankingSystem.Contracts.Interfaces.IExternalServices;
using BankingSystem.Contracts.Interfaces.IRepositories;
using BankingSystem.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Moq;

using System.Data;
using System.Net.NetworkInformation;
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
        public async Task CreateTransactionAsync_ShouldCreate()
        {
            var bankProfit = 1;
            var amount = 10;
            var fromAccountId = 1;
            var toAccountId = 2;
            var currency = "GEL";
            var isATM = false;

            _mockUnitOfWork.Setup(u => u.TransactionDetailsRepository.CreateTransactionAsync(It.IsAny<TransactionDetails>())).ReturnsAsync(1);

            var response = await _transactionDetailsService.CreateTransactionAsync(bankProfit, amount, fromAccountId, toAccountId, currency);

            Assert.True(response.Success);
            Assert.Equal("Transaction was successfull!", response.Message);
            Assert.Equal(200, response.StatusCode);

            _mockUnitOfWork.Verify(u => u.SaveChanges(), Times.Once());
            _mockUnitOfWork.Verify(u => u.TransactionDetailsRepository, Times.Exactly(2));
        }

        [Fact]
        public async Task CreateTransactionAsync_ShouldNotCreate()
        {
            var bankProfit = 1;
            var amount = 10;
            var fromAccountId = 1;
            var toAccountId = 1;
            var currency = "GEL";
            var isATM = true;

            _mockUnitOfWork.Setup(u => u.TransactionDetailsRepository.CreateTransactionAsync(It.IsAny<TransactionDetails>())).ReturnsAsync(0);

            var response = await _transactionDetailsService.CreateTransactionAsync(bankProfit, amount, fromAccountId, toAccountId, currency);

            Assert.False(response.Success);
            Assert.Equal("Transaction could not be created, something happened!", response.Message);
            Assert.Equal(400, response.StatusCode);

            _mockUnitOfWork.Verify(u => u.SaveChanges(), Times.Never());
            _mockUnitOfWork.Verify(u => u.TransactionDetailsRepository, Times.Exactly(2));
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
        public async Task BankProfitByTimePeriodAsync_ShouldReturnValue()
        {
            var bankProfitDtos = new List<BankProfitDTO>();
            _mockUnitOfWork.Setup(u => u.TransactionDetailsRepository.GetBankProfitByTimePeriodAsync()).ReturnsAsync(bankProfitDtos);

            var response = await _transactionDetailsService.BankProfitByTimePeriodAsync();

            Assert.True(response.Success);
            Assert.Equal("Bank profit retrieved successfully.", response.Message);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(bankProfitDtos, response.Data);

            _mockUnitOfWork.Verify(u => u.TransactionDetailsRepository, Times.Once());
        }


        [Fact]
        public async Task BankProfitByTimePeriodAsync_ShouldNotReturnValue()
        {
            List<BankProfitDTO> bankProfitDtos = null;
            _mockUnitOfWork.Setup(u => u.TransactionDetailsRepository.GetBankProfitByTimePeriodAsync()).ReturnsAsync(bankProfitDtos);

            var response = await _transactionDetailsService.BankProfitByTimePeriodAsync();

            Assert.False(response.Success);
            Assert.Equal("No bank profit data found.", response.Message);
            Assert.Equal(404, response.StatusCode);
            Assert.Null(response.Data);

            _mockUnitOfWork.Verify(u => u.TransactionDetailsRepository, Times.Once());
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
        public async Task NumberOfTransactionsChartAsync_ShouldReturnValue()
        {
            var transactionCountChartDtos = new List<TransactionCountChartDTO>();
            _mockUnitOfWork.Setup(u => u.TransactionDetailsRepository.NumberOfTransactionsLastMonthAsync()).ReturnsAsync(transactionCountChartDtos);

            var response = await _transactionDetailsService.NumberOfTransactionsChartAsync();

            Assert.True(response.Success);
            Assert.Equal("Transaction Count retreived!", response.Message);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(transactionCountChartDtos, response.Data);

            _mockUnitOfWork.Verify(u => u.TransactionDetailsRepository, Times.Once());
        }

        [Fact]
        public async Task NumberOfTransactionsChartAsync_ShouldNotReturnValue()
        {
            List<TransactionCountChartDTO> transactionCountChartDtos = null;
            _mockUnitOfWork.Setup(u => u.TransactionDetailsRepository.NumberOfTransactionsLastMonthAsync()).ReturnsAsync(transactionCountChartDtos);

            var response = await _transactionDetailsService.NumberOfTransactionsChartAsync();

            Assert.False(response.Success);
            Assert.Equal("Transaction Count couldn't be retreived!", response.Message);
            Assert.Equal(404, response.StatusCode);
            Assert.Null(response.Data);

            _mockUnitOfWork.Verify(u => u.TransactionDetailsRepository, Times.Once());
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
        public async Task TotalIncomeExpenseAsync_ShouldReturnValue()
        {
            var dateRangeDto = new DateRangeDTO() { FromDate = DateTime.Now.AddMonths(-1), ToDate = DateTime.Now.AddDays(-1) };
            string email = "t@gmail.com";
            var incomeExpense = new IncomeExpenseDTO
            {
                Income = new Dictionary<string, decimal> { { "GEL", 200.65m } },
                Expense = new Dictionary<string, decimal> { { "GEL", 121.65m } }
            };

            _mockUnitOfWork.Setup(u => u.TransactionDetailsRepository.GetTotalIncomeAsync(dateRangeDto.FromDate, dateRangeDto.ToDate, email)).ReturnsAsync(incomeExpense.Income);
            _mockUnitOfWork.Setup(u => u.TransactionDetailsRepository.GetTotalExpenseAsync(dateRangeDto.FromDate, dateRangeDto.ToDate, email)).ReturnsAsync(incomeExpense.Expense);

            var response = await _transactionDetailsService.TotalIncomeExpenseAsync(dateRangeDto,email);

            Assert.True(response.Success);
            Assert.Equal("Income and Expense retreived!", response.Message);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(incomeExpense.Income, response.Data.Income);
            Assert.Equal(incomeExpense.Expense, response.Data.Expense);

            _mockUnitOfWork.Verify(u => u.TransactionDetailsRepository, Times.Exactly(2));
        }

        [Fact]
        public async Task TotalIncomeExpenseAsync_ShouldNotReturnValue()
        {
            var dateRangeDto = new DateRangeDTO() { FromDate = DateTime.Now.AddDays(-1), ToDate = DateTime.Now.AddMonths(-1) };
            string email = "t@gmail.com";

            var response = await _transactionDetailsService.TotalIncomeExpenseAsync(dateRangeDto, email);

            Assert.False(response.Success);
            Assert.Equal("Provide correct time, toDate cannot be yearlier than fromDate!", response.Message);
            Assert.Equal(400, response.StatusCode);
            Assert.Null(response.Data);

            _mockUnitOfWork.Verify(u => u.TransactionDetailsRepository, Times.Never());
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

        [Fact]
        public async Task CalculateATMWithdrawalTransactionAsync_ShouldReturnValue()
        {
            string cardNumber = "4998892941729115";
            string pin = "1234";
            decimal withdrawalAmount = 200;
            string withdrawalCurrency = "GEL";
            var accountInfo = new BalanceAndWithdrawalDTO() { Amount = 1200, Currency = "USD", WithdrawnAmountIn24Hours = 200 };

            _mockUnitOfWork.Setup(u => u.CardRepository.GetBalanceAndWithdrawnAmountAsync(cardNumber, pin)).ReturnsAsync(accountInfo);
            _mockExchangeRateService.Setup(er => er.GetCurrencyRateAsync(withdrawalCurrency, accountInfo.Currency)).ReturnsAsync(0.333m);

            Mock<IConfigurationSection> mockSectionATMPercent = new Mock<IConfigurationSection>();
            mockSectionATMPercent.Setup(x => x.Value).Returns("2");
            _mockConfiguration.Setup(x => x.GetSection("TransactionFees:AtmWithdrawalPercent")).Returns(mockSectionATMPercent.Object);

            Mock<IConfigurationSection> mockSectionATMLimit = new Mock<IConfigurationSection>();
            mockSectionATMLimit.Setup(x => x.Value).Returns("10000");
            _mockConfiguration.Setup(x => x.GetSection("TransactionFees:AtmWithdrawalLimitForDay")).Returns(mockSectionATMLimit.Object);

            var response = await _transactionDetailsService.CalculateATMWithdrawalTransactionAsync(cardNumber, pin, withdrawalAmount, withdrawalCurrency);

            Assert.True(response.Success);
            Assert.Equal(200, response.StatusCode);
            Assert.NotNull(response.Data);

            _mockUnitOfWork.Verify(u => u.CardRepository, Times.Once());
        }

        [Fact]
        public async Task CalculateATMWithdrawalTransactionAsync_ShouldNotReturnValue()
        {
            string cardNumber = "1233367899931777";
            string pin = "1111";
            decimal withdrawalAmount = 444;
            string withdrawalCurrency = "EUR";
            BalanceAndWithdrawalDTO accountInfo = null;

            _mockUnitOfWork.Setup(u => u.CardRepository.GetBalanceAndWithdrawnAmountAsync(cardNumber, pin)).ReturnsAsync(accountInfo);
            
            var response = await _transactionDetailsService.CalculateATMWithdrawalTransactionAsync(cardNumber, pin, withdrawalAmount, withdrawalCurrency);

            Assert.False(response.Success);
            Assert.Equal("Unable to retrieve account details.", response.Message);
            Assert.Null(response.Data);
            Assert.Equal(400, response.StatusCode);

            _mockUnitOfWork.Verify(u => u.CardRepository, Times.Once());
        }
    }
}
