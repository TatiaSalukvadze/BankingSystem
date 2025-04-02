using BankingSystem.Contracts.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystem.API.Controllers.Reports
{
    [Authorize(policy: "ManagerOnly")]
    [Route("/Report/[controller]")]
    public class TransactionStatisticsController : WrapperController
    {
        private readonly ITransactionDetailsService _transactionService;

        public TransactionStatisticsController(ITransactionDetailsService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet("Count")]
        public async Task<IActionResult> TransactionsCount()
        {
            var response = await _transactionService.NumberOfTransactionsAsync();
            return new ObjectResult(response);
        }

        [HttpGet("BankProfitByTimePeriod")] 
        public async Task<IActionResult> GetBankProfitByTimePeriodAsync()
        {
            var response = await _transactionService.BankProfitByTimePeriodAsync();
            return new ObjectResult(response);
        }

        [HttpGet("CountChart")]
        public async Task<IActionResult> TransactionsCountChart()
        {
            var response = await _transactionService.NumberOfTransactionsChartAsync();
            return new ObjectResult(response);
        }

        [HttpGet("AverageProfit")]
        public async Task<IActionResult> AverageProfit()
        {
            var response = await _transactionService.AverageBankProfitAsync();
            return new ObjectResult(response);
        }

        [HttpGet("TotalAtmWithdrawals")]
        public async Task<IActionResult> TotalAtmWithdrawals()
        {
            var response = await _transactionService.TotalAtmWithdrawalsAsync();
            return new ObjectResult(response);
        }
    }
}
