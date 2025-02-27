using BankingSystem.Application.Services;
using BankingSystem.Contracts.DTOs.ATM;
using BankingSystem.Contracts.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BankingSystem.API.Controllers.ATM
{
    [Route("/ATM/[controller]")]
    public class CardController : WrapperController
    {

        private readonly ICardService _cardService;

        public CardController(ICardService cardService)
        {
            _cardService = cardService;
        }

        //tamar
        [HttpGet("SeeBalance")]
        public async Task<IActionResult> SeeBalanceAsync([FromQuery] string cardNumber, [FromQuery] string pin)
        {
            var (success, message, data) = await _cardService.SeeBalanceAsync(cardNumber, pin);
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
            var (success, message) = await _cardService.WithdrawAsync(withdrawalDto);
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
          
            var (success, message) = await _cardService.ChangeCardPINAsync(changeCardDtp);
            return await HandleResult(success, message);
            //if (!success)
            //{
            //    return BadRequest(message);
            //}
            //return Ok(new { message });
        }
    }
}
