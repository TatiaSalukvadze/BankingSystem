using BankingSystem.Application.Services;
using BankingSystem.Contracts.DTOs.ATM;
using BankingSystem.Contracts.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BankingSystem.API.Controllers
{
    [Route("/[controller]")]
    public class ATMController : WrapperController
    {
        private readonly ITransactionOperationService _transactionOperationService;
        private readonly ICardService _cardService;

        public ATMController(ITransactionOperationService transactionOperationService, ICardService cardService)
        {
            _transactionOperationService = transactionOperationService;
            _cardService = cardService;
        }

        //tamar
        [HttpGet("SeeBalance")]
        public async Task<IActionResult> SeeBalanceAsync([FromQuery] CardAuthorizationDTO cardAuthorizationDto)// string cardNumber, [FromQuery] string pin)
        {
            var response = await _cardService.SeeBalanceAsync(cardAuthorizationDto);
            var (success, message, data) = (response.Success, response.Message, response.Data);
            return await HandleResult(success, message, data);
            //if (!success)
            //{
            //    return BadRequest(new { message });
            //}
            //return Ok(new { message, data });
        }

        //tamar
        [HttpPost("Withdraw")]
        public async Task<IActionResult> WithdrawAsync([FromForm] WithdrawalDTO withdrawalDto)
        {
            var response = await _transactionOperationService.WithdrawAsync(withdrawalDto);
            var (success, message) = (response.Success, response.Message);
            return await HandleResult(success, message);
            //if (!success)
            //{
            //    return BadRequest(new { message });
            //}

            //return Ok(new { message });
        }

        //tatia
        [HttpPut("PIN")]
        public async Task<IActionResult> ChangeCardPINAsync([FromForm] ChangeCardPINDTO changeCardDtp)
        {
            var response = await _cardService.ChangeCardPINAsync(changeCardDtp);
            var (success, message) = (response.Success, response.Message);
            return await HandleResult(success, message);
            //if (!success)
            //{
            //    return BadRequest(message);
            //}
            //return Ok(new { message });
        }
    }
}
