using BankingSystem.Contracts.DTOs.ATM;
using BankingSystem.Contracts.DTOs.Report;
using BankingSystem.Contracts.DTOs.UserBanking;
using BankingSystem.Contracts.Interfaces;
using BankingSystem.Contracts.Interfaces.IExternalServices;
using BankingSystem.Contracts.Interfaces.IServices;
using BankingSystem.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace BankingSystem.Application.Services
{
    public class TransactionDetailsService : ITransactionDetailsService
    {
        private readonly IExchangeRateService _exchangeRateService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public TransactionDetailsService(IConfiguration configuration, IUnitOfWork unitOfWork, IExchangeRateService exchangeRateService)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _exchangeRateService = exchangeRateService;
        }

        //both
        public async Task<(bool Success, string Message)> CreateTransactionAsync(decimal bankProfit, 
            decimal amount, int fromAccountId, int toAccountId, string currency, bool IsATM = false)
        {
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
                return (false, "Transaction could not be created, something happened!");
            }

            transaction.Id = insertedId;
            _unitOfWork.SaveChanges();
            return (true, "Transaction was successfull!");
        }
        
        //tatia
        public async Task<(bool Success, string Message, TransactionCountDTO Data)> NumberOfTransactionsAsync()
        {
            TransactionCountDTO transactionCountDTO = await _unitOfWork.TransactionDetailsRepository.NumberOfTransactionsAsync();
            if (transactionCountDTO is not null)
                return (true, "Transaction Count retreived!", transactionCountDTO);
            else return (false, "Transaction Count couldn't be retreived!", null);
        }

        //tamar
        public async Task<(bool Success, string Message, List<BankProfitDTO> Data)> GetBankProfitByTimePeriodAsync()
        {
            List<BankProfitDTO> profitData = await _unitOfWork.TransactionDetailsRepository.GetBankProfitByTimePeriodAsync();

            if (profitData.Any())
            {
                return (true, "Bank profit retrieved successfully.", profitData);
            }

            return (false, "No bank profit data found.", null);
        }

        //tatia
        public async Task<(bool Success, string Message, Dictionary<string, decimal> Data)> AverageBankProfitAsync()
        {
            var averageBankProfits = await _unitOfWork.TransactionDetailsRepository.AverageBankProfitAsyncAsync();
            if (averageBankProfits is not null && averageBankProfits.Count != 0)
                return (true, "Bank profit Count retreived!", averageBankProfits);
            else return (false, "Bank profit couldn't be retreived!", null);
        }

        //tatia
        public async Task<(bool Success, string Message, List<TransactionCountChartDTO> Data)> NumberOfTransactionsChartAsync()
        {
            List<TransactionCountChartDTO> transactionCountDTO = await _unitOfWork.TransactionDetailsRepository.NumberOfTransactionsLastMonthAsync();
            if (transactionCountDTO is not null)
                return (true, "Transaction Count retreived!", transactionCountDTO);
            else return (false, "Transaction Count couldn't be retreived!", null);
        }

        //tamr
        public async Task<(bool Success, string Message, List<TotalAtmWithdrawalDTO> Data)> GetTotalAtmWithdrawalsAsync()
        {
            var rawData = await _unitOfWork.TransactionDetailsRepository.GetTotalAtmWithdrawalsAsync();

            if (rawData != null && rawData.Any())
            {
                return (true, "ATM withdrawals data retrieved successfully.", rawData);
            }

            return (false, "No ATM withdrawal data found.", null);
        }

        //both
        public async Task<(bool Success, string Message, IncomeExpenseDTO Data)> TotalIncomeExpenseAsync(DateRangeDTO dateRangeDto, string email)
        {
            var exactNow = DateTime.Now;
            var now = new DateTime(exactNow.Year, exactNow.Month, exactNow.Day, exactNow.Hour, exactNow.Minute, 0, exactNow.Kind);
            if (dateRangeDto.FromDate >= now || dateRangeDto.ToDate > now)
            {
                return (false, "Provide correct time, not future time!", null);
            }
            if(dateRangeDto.ToDate <= dateRangeDto.FromDate)
            {
                return (false, "Provide correct time, toDate cannot be yearlier than fromDate!", null);
            }
            var income = await _unitOfWork.TransactionDetailsRepository.GetTotalIncomeAsync(dateRangeDto.FromDate, dateRangeDto.ToDate, email);
            var expense = await _unitOfWork.TransactionDetailsRepository.GetTotalExpenseAsync(dateRangeDto.FromDate, dateRangeDto.ToDate, email);
            var incomeExpense = new IncomeExpenseDTO
            {
                Income = income,
                Expense = expense
            };
            return (true, "Income and Expense retreived!", incomeExpense);
        }

        #region transactionHelpers
        //tatia
        public async Task<(bool Success, string Message, TransferAmountCalculationDTO Data)> CalculateTransferAmountAsync(
            string fromCurrency, string toCurrency, decimal amountToTransfer, bool isSelfTransfer)
        {
            decimal currencyRate = await _exchangeRateService.GetCurrencyRateAsync(fromCurrency, toCurrency);
            if (currencyRate <= 0) { return (false, "One of the account has incorrect currency!", null); }

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
            return (true,"", transferAmounts);
        }

        //tamr
        //for atm
        public async Task<(bool success, string message, AtmWithdrawalCalculationDTO Data)> CalculateATMWithdrawalTransactionAsync(string cardNumber, string pin, decimal withdrawalAmount, string withdrawalCurrency)
        {
            var accountInfo = await _unitOfWork.CardRepository.GetBalanceAndWithdrawnAmountAsync(cardNumber, pin);

            if (accountInfo == null)
            {
                return (false, "Unable to retrieve account details.", null);
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
                    return (false, "Currency conversion failed.", null);
                }

                convertedAmount *= exchangeRate;
            }

            decimal atmWithdrawalPercent = _configuration.GetValue<decimal>("TransactionFees:AtmWithdrawalPercent");
            decimal fee = convertedAmount * (atmWithdrawalPercent / 100);
            decimal totalAmountToDeduct = convertedAmount + fee;

            if (accountBalance < totalAmountToDeduct)
            {
                return (false, "You don't have enough money", null);
            }

            decimal newTotalWithdrawnIn24Hours = totalWithdrawnIn24Hours + totalAmountToDeduct;
            decimal atmWithdrawalLimit = _configuration.GetValue<decimal>("TransactionFees:AtmWithdrawalLimitForDay");

            if (newTotalWithdrawnIn24Hours > atmWithdrawalLimit)
            {
                return (false, $"You can't withdraw more than {atmWithdrawalLimit} {accountCurrency} within 24 hours.", null);
            }

            var withdrawalData = new AtmWithdrawalCalculationDTO
            {
                Fee = fee,
                Balance = convertedAmount,
                Currency = accountCurrency,
                TotalAmountToDeduct = totalAmountToDeduct
            };

            return (true, "", withdrawalData);
        }
        #endregion
    }
}

//tatia
//private async Task<decimal> CalculateCurrencyRateAsync(CurrencyType fromCurrency, CurrencyType toCurrency)
//{
//    decimal currencyRate = 1;

//    if (fromCurrencyId != toCurrencyId)
//    {
//        var fromCurrency = await _unitOfWork.CurrencyRepository.FindTypeByIdAsync(fromCurrencyId);
//        // ((CurrencyType)fromCurrencyId).ToString();
//        var toCurrency = await _unitOfWork.CurrencyRepository.FindTypeByIdAsync(toCurrencyId); //((CurrencyType)toCurrencyId).ToString();
//        if (fromCurrency == "" || toCurrency == "") { return 0; }
//        currencyRate = await _exchangeRateService.GetCurrencyRateAsync(fromCurrency, toCurrency);
//    }
//    return currencyRate;
//}

//}
////tatia
//private async Task<bool> UpdateAccountsAmountAsync(int fromAccountId, int toAccountId, 
//    decimal amountFromAccount, decimal amountToAccount)
//{
//    var fromAccountUpdated = await _unitOfWork.AccountRepository.UpdateAccountAmountAsync(fromAccountId, -amountFromAccount);
//    var toAccountUpdated = await _unitOfWork.AccountRepository.UpdateAccountAmountAsync(toAccountId, amountToAccount);

//    if (!fromAccountUpdated || !toAccountUpdated)
//    {
//        return false; 
//    }
//    return true;

//}

//tatia
//public async Task<(bool Success, string Message, TransactionDetails Data)> OnlineTransactionAsync(CreateTransactionDTO createTransactionDto,
//    string email, bool isSelfTransfer)
//{
//    var (bankProfit, amountFromAccount, amountToAccount) = await CalculateTransactionAmountAsync(fromAccount.CurrencyId,
//        toAccount.CurrencyId, createTransactionDto.Amount, isSelfTransfer);
//    if (bankProfit == 0 && amountFromAccount == 0 && amountToAccount == 0) {
//        return (false, "One of the account has incorrect currency!", null);
//    }
//    if (fromAccount.Amount < amountFromAccount)
//    {
//        return (false, "You don't have enough money to transfer on your account!", null);
//    }
//    bool accountsUpdated = await UpdateAccountsAmountAsync(fromAccount.Id, toAccount.Id, amountFromAccount, amountToAccount);
//    if(!accountsUpdated) return (false, "Balance couldn't be updated!", null);

//    var transaction = new TransactionDetails
//    {
//        BankProfit = bankProfit,
//        Amount = createTransactionDto.Amount,
//        FromAccountId = fromAccount.Id,
//        ToAccountId = toAccount.Id,
//        CurrencyId = fromAccount.CurrencyId,
//        IsATM = false,
//    };

//    int insertedId = await _unitOfWork.TransactionDetailsRepository.CreateTransactionAsync(transaction);
//    if (insertedId <= 0)
//    {
//        return (false, "Transaction could not be created, something happened!", null);
//    }

//    transaction.Id = insertedId;
//    _unitOfWork.SaveChanges();
//    return (true, "Transaction was successfull!", transaction);


//}
//helper methods
//tatia
//private async Task<(bool Validated, string Message, Account from, Account to)> ValidateAccountsAsync(string fromIBAN,
//    string toIBAN, string email, bool isSelfTransfer)
//{
//    if (fromIBAN == toIBAN)
//    {
//        return (false, "You can't transfer to same account!", null, null);
//    }
//    var fromAccount = await _accountRepository.FindAccountByIBANandEmailAsync(fromIBAN, email);
//    Account toAccount;
//    if (isSelfTransfer)
//    {
//        toAccount = await _accountRepository.FindAccountByIBANandEmailAsync(toIBAN, email);
//    }
//    else
//    {
//        toAccount = await _accountRepository.FindAccountByIBANAsync(toIBAN);
//    }
//    if (fromAccount is null || toAccount is null)
//    {
//        return (false, "There is no account for one or both provided IBANs, check well!", null, null);
//    }

//    //if (fromAccount.Amount < amountToTransfer)
//    //{
//    //    return (false, "You don't have enough money to transfer on your account!", null, null);
//    //}
//    return (true, "Accounts validated!", fromAccount, toAccount);
//}