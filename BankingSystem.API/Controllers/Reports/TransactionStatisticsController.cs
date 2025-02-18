using BankingSystem.Contracts.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystem.API.Controllers.Reports
{
    [Route("/Report/[controller]")]
    public class TransactionStatisticsController : WrapperController
    {


        private readonly ITransactionService _transactionService;

        public TransactionStatisticsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }


        //[Authorize(policy: "ManagerOnly")]
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
