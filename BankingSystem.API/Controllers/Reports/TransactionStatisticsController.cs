using BankingSystem.Application.Services;
using BankingSystem.Contracts.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystem.API.Controllers.Reports
{
    //[Authorize(policy: "ManagerOnly")]
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

            var (success, message, data) = await _transactionService.NumberOfTransactionsAsync();
            return await HandleResult(success, message, data);
            //if (!success)
            //{
            //    return BadRequest(new { message });
            //}

            //return Ok(new { message, data });
        }

        [HttpGet("BankProfitByTimePeriod")] //and currency
        public async Task<IActionResult> GetBankProfitByTimePeriodAsync()
        {
            var (success, message, data) = await _transactionService.GetBankProfitByTimePeriodAsync();
            return await HandleResult(success, message, data);
            //if (!success)
            //    return BadRequest(new { message });

            //return Ok(new { message, data });
        }


        [HttpGet("CountChart")]
        public async Task<IActionResult> TransactionsCountChart()
        {

            var (success, message, data) = await _transactionService.NumberOfTransactionsChartAsync();
            return await HandleResult(success, message, data);
            //if (!success)
            //{
            //    return BadRequest(new { message });
            //}

            //return Ok(new { message, data });
        }

        [HttpGet("AverageProfit")]
        public async Task<IActionResult> AverageProfit()
        {

            var (success, message, data) = await _transactionService.AverageBankProfitAsync();
            return await HandleResult(success, message, data);
            //if (!success)
            //{
            //    return BadRequest(new { message });
            //}

            //return Ok(new { message, data });
        }

        [HttpGet("AtmWithdrawals")]
        public async Task<IActionResult> GetAtmWithdrawals()
        {
            var (success, message, data) = await _transactionService.GetTotalAtmWithdrawalsAsync();
            return await HandleResult(success, message, data);
            //if (!success)
            //{
            //    return BadRequest(new { message });
            //}

            //return Ok(new { message, data });
        }
    }
}
