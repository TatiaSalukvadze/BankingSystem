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

            var response = await _transactionService.NumberOfTransactionsAsync();
            var (success, message, data) = (response.Success, response.Message, response.Data);
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
            var response = await _transactionService.GetBankProfitByTimePeriodAsync();
            var (success, message, data) = (response.Success, response.Message, response.Data);
            return await HandleResult(success, message, data);
            //if (!success)
            //    return BadRequest(new { message });

            //return Ok(new { message, data });
        }


        [HttpGet("CountChart")]
        public async Task<IActionResult> TransactionsCountChart()
        {

            var response = await _transactionService.NumberOfTransactionsChartAsync();
            var (success, message, data) = (response.Success, response.Message, response.Data);
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

            var response = await _transactionService.AverageBankProfitAsync();
            var (success, message, data) = (response.Success, response.Message, response.Data);
            return await HandleResult(success, message, data);
            //if (!success)
            //{
            //    return BadRequest(new { message });
            //}

            //return Ok(new { message, data });
        }

        [HttpGet("TotalAtmWithdrawals")]
        public async Task<IActionResult> GetAtmWithdrawals()
        {
            var response = await _transactionService.GetTotalAtmWithdrawalsAsync();
            var (success, message, data) = (response.Success, response.Message, response.Data);
            return await HandleResult(success, message, data);
            //if (!success)
            //{
            //    return BadRequest(new { message });
            //}

            //return Ok(new { message, data });
        }
    }
}
