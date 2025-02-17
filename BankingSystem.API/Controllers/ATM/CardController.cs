using BankingSystem.Application.Services;
using BankingSystem.Contracts.DTOs;
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

        //[HttpGet("balance")]
        //public async Task<IActionResult> SeeBalance([FromQuery] string cardNumber)
        //{
        //    try
        //    {
        //        var (success, message, balance) = await _cardService.GetBalanceAsync(cardNumber);

        //        if (!success)
        //        {
        //            return BadRequest(message);
        //        }

        //        return Ok(new { Balance = balance });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Internal server error: {ex.Message}");
        //    }
        //}


        [HttpPut("PIN")]
        public async Task<IActionResult> ChangeCardPINAsync([FromForm] ChangeCardPINDTO changeCardDtp)
        {
          
            var (success, message) = await _cardService.ChangeCardPINAsync(changeCardDtp);
            if (!success)
            {
                return BadRequest(message);
            }
            return Ok(new { message });
        }
    }
}
