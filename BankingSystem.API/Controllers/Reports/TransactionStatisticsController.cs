using BankingSystem.Contracts.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystem.API.Controllers.Reports
{
    [Authorize(policy: "ManagerOnly")]
    [Route("/Report/[controller]")]
    public class TransactionStatisticsController : WrapperController
    {


        private readonly ITransactionService _transactionService;

        public TransactionStatisticsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet("Count")]
        public async Task<IActionResult> TransactionsCount()
        {

            var (success, message, data) = await _transactionService.NumberOfTransactionsAsync();

            if (!success)
            {
                return BadRequest(new { message });
            }

            return Ok(new { message, data });
        }

        [HttpGet("CountChart")]
        public async Task<IActionResult> TransactionsCountChart()
        {

            var (success, message, data) = await _transactionService.NumberOfTransactionsChartAsync();

            if (!success)
            {
                return BadRequest(new { message });
            }

            return Ok(new { message, data });
        }

        [HttpGet("AverageProfit")]
        public async Task<IActionResult> AverageProfit()
        {

            var (success, message, data) = await _transactionService.AverageBankProfitAsync();

            if (!success)
            {
                return BadRequest(new { message });
            }

            return Ok(new { message, data });
        }
    }
}
