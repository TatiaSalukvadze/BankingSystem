using BankingSystem.Contracts.DTOs.ATM;
using BankingSystem.Contracts.DTOs.Report;
using BankingSystem.Contracts.DTOs.UserBanking;
using BankingSystem.Contracts.Interfaces;
using BankingSystem.Contracts.Interfaces.IExternalServices;
using BankingSystem.Contracts.Interfaces.IServices;
using BankingSystem.Contracts.Response;
using BankingSystem.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace BankingSystem.Application.Services
{
    public class TransactionDetailsService : ITransactionDetailsService
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IExchangeRateService _exchangeRateService;

        public TransactionDetailsService(IConfiguration configuration, IUnitOfWork unitOfWork, IExchangeRateService exchangeRateService)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _exchangeRateService = exchangeRateService;
        }

        public async Task<SimpleResponse> CreateTransactionAsync(decimal bankProfit, 
            decimal amount, int fromAccountId, int toAccountId, string currency, bool IsATM = false)
        {
            var response = new SimpleResponse();
            var transaction = new TransactionDetails
            {
                BankProfit = bankProfit,
                Amount = amount,
                FromAccountId = fromAccountId,
                ToAccountId = toAccountId,
                Currency = currency,
                IsATM = IsATM,
            };

            _unitOfWork.TransactionDetailsRepository.SetTransaction(_unitOfWork.Transaction());
            int insertedId = await _unitOfWork.TransactionDetailsRepository.CreateTransactionAsync(transaction);
            if (insertedId <= 0)
            {
                return response.Set(false, "Transaction could not be created, something happened!", 400);
            }
            transaction.Id = insertedId;
            _unitOfWork.SaveChanges();

            return response.Set(true, "Transaction was successfull!", 200);
        }
        
        public async Task<Response<TransactionCountDTO>> NumberOfTransactionsAsync()
        {
            var response = new Response<TransactionCountDTO>(); 
            TransactionCountDTO transactionCountDTO = await _unitOfWork.TransactionDetailsRepository.NumberOfTransactionsAsync();
            if (transactionCountDTO is null)
            {
                return response.Set(false, "Transaction Count couldn't be retreived!", null, 404); 
            }

            return response.Set(true, "Transaction Count retreived!", transactionCountDTO, 200);
        }

        public async Task<Response<List<BankProfitDTO>>> BankProfitByTimePeriodAsync()
        {
            var response = new Response<List<BankProfitDTO>>();
            List<BankProfitDTO> profitData = await _unitOfWork.TransactionDetailsRepository.GetBankProfitByTimePeriodAsync();
            if (profitData is null)
            {
                return response.Set(false, "No bank profit data found.", null, 404);
            }

            return response.Set(true, "Bank profit retrieved successfully.", profitData, 200);
        }

        public async Task<Response<Dictionary<string, decimal>>> AverageBankProfitAsync()
        {
            var response = new Response<Dictionary<string, decimal>>();
            var averageBankProfits = await _unitOfWork.TransactionDetailsRepository.AverageBankProfitAsync();
            if (averageBankProfits == null || averageBankProfits.Count == 0)
            {
                return response.Set(false, "Bank profit couldn't be retrieved!", null, 404);
            }

            return response.Set(true, "Bank profit count retrieved!", averageBankProfits, 200);
        }


        public async Task<Response<List<TransactionCountChartDTO>>> NumberOfTransactionsChartAsync()
        {
            var response = new Response<List<TransactionCountChartDTO>>();
            List<TransactionCountChartDTO> transactionCountDTO = await _unitOfWork.TransactionDetailsRepository.NumberOfTransactionsLastMonthAsync();
            if (transactionCountDTO is null)
            {
                return response.Set(false, "Transaction Count couldn't be retreived!", null, 404);
            }

            return response.Set(true, "Transaction Count retreived!", transactionCountDTO, 200);
        }

        public async Task<Response<List<TotalAtmWithdrawalDTO>>> TotalAtmWithdrawalsAsync()
        {
            var response = new Response<List<TotalAtmWithdrawalDTO>>();
            var rawData = await _unitOfWork.TransactionDetailsRepository.GetTotalAtmWithdrawalsAsync();
            if (rawData is null)
            {
                return response.Set(false, "No ATM withdrawal data found.", null, 404);
            }

            return response.Set(true, "ATM withdrawals data retrieved successfully.", rawData, 200);
        }

        public async Task<Response<IncomeExpenseDTO>> TotalIncomeExpenseAsync(DateRangeDTO dateRangeDto, string email)
        {
            var response = new Response<IncomeExpenseDTO>();
            var now = DateTime.Now;
            //var now = new DateTime(exactNow.Year, exactNow.Month, exactNow.Day, exactNow.Hour, exactNow.Minute, 0, exactNow.Kind);

            if (dateRangeDto.FromDate >= now || dateRangeDto.ToDate > now)
            {
                return response.Set(false, "Provide correct time, not future time!", null, 400);
            }
            if(dateRangeDto.ToDate <= dateRangeDto.FromDate)
            {
                return response.Set(false, "Provide correct time, toDate cannot be yearlier than fromDate!", null, 400);
            }

            var income = await _unitOfWork.TransactionDetailsRepository.GetTotalIncomeAsync(dateRangeDto.FromDate, dateRangeDto.ToDate, email);
            var expense = await _unitOfWork.TransactionDetailsRepository.GetTotalExpenseAsync(dateRangeDto.FromDate, dateRangeDto.ToDate, email);
            
            var incomeExpense = new IncomeExpenseDTO
            {
                Income = income,
                Expense = expense
            };

            return response.Set(true, "Income and Expense retreived!", incomeExpense, 200);
        }

        #region transactionHelpers
        public async Task<Response<TransferAmountCalculationDTO>> CalculateTransferAmountAsync(
            string fromCurrency, string toCurrency, decimal amountToTransfer, bool isSelfTransfer)
        {
            var response = new Response<TransferAmountCalculationDTO>();
            decimal currencyRate = await _exchangeRateService.GetCurrencyRateAsync(fromCurrency, toCurrency);
            if (currencyRate <= 0) 
            { 
                return response.Set(false, "One of the account has incorrect currency!", null, 400); 
            }

            decimal fee;
            decimal extraFeeValue = 0;
            if (isSelfTransfer)
            {
                fee = _configuration.GetValue<decimal>("TransactionFees:SelfTransferPercent");
            }
            else
            {
                fee = _configuration.GetValue<decimal>("TransactionFees:StandartTransferPercent");
                extraFeeValue = _configuration.GetValue<decimal>("TransactionFees:StandartTransferValue");
            }

            var transferAmounts = new TransferAmountCalculationDTO();
            transferAmounts.BankProfit = amountToTransfer * fee / 100 + extraFeeValue;
            transferAmounts.AmountFromAccount = amountToTransfer + transferAmounts.BankProfit;
            transferAmounts.AmountToAccount = amountToTransfer * currencyRate;

            return response.Set(true, "", transferAmounts, 200);
        }

        public async Task<Response<AtmWithdrawalCalculationDTO>> CalculateATMWithdrawalTransactionAsync(string cardNumber, string pin, decimal withdrawalAmount, string withdrawalCurrency)
        {
            var response = new Response<AtmWithdrawalCalculationDTO>();
            var accountInfo = await _unitOfWork.CardRepository.GetBalanceAndWithdrawnAmountAsync(cardNumber, pin);
            if (accountInfo == null)
            {
                return response.Set(false, "Unable to retrieve account details.", null, 400);
            }

            decimal accountBalance = accountInfo.Amount;
            decimal totalWithdrawnIn24Hours = accountInfo.WithdrawnAmountIn24Hours;
            string accountCurrency = accountInfo.Currency;
      
            decimal convertedAmount = withdrawalAmount;
            if (withdrawalCurrency != accountCurrency)
            {
                decimal exchangeRate = await _exchangeRateService.GetCurrencyRateAsync(withdrawalCurrency, accountCurrency);

                if (exchangeRate <= 0)
                {
                    return response.Set(false, "Currency conversion failed.", null, 400);
                }

                convertedAmount *= exchangeRate;
            }

            decimal atmWithdrawalPercent = _configuration.GetValue<decimal>("TransactionFees:AtmWithdrawalPercent");
            decimal fee = convertedAmount * (atmWithdrawalPercent / 100);
            decimal totalAmountToDeduct = convertedAmount + fee;
            if (accountBalance < totalAmountToDeduct)
            {
                return response.Set(false, "You don't have enough money", null, 400);
            }

            decimal newTotalWithdrawnIn24Hours = totalWithdrawnIn24Hours + totalAmountToDeduct;
            decimal atmWithdrawalLimit = _configuration.GetValue<decimal>("TransactionFees:AtmWithdrawalLimitForDay");
            if (newTotalWithdrawnIn24Hours > atmWithdrawalLimit)
            {
                return response.Set(false, $"You can't withdraw more than {atmWithdrawalLimit} {accountCurrency} within 24 hours.", null, 400);
            }

            var withdrawalData = new AtmWithdrawalCalculationDTO
            {
                Fee = fee,
                Balance = convertedAmount,
                Currency = accountCurrency,
                TotalAmountToDeduct = totalAmountToDeduct
            };

            return response.Set(true, "", withdrawalData, 200);
        }
        #endregion
    }
}
