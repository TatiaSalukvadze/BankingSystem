using BankingSystem.Contracts.DTOs;
using BankingSystem.Contracts.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BankingSystem.API.Controllers.OnlineBank
{
    [Route("/OnlineBank/[controller]")]
    public class PersonController : WrapperController
    {
        private readonly IPersonService _personService;
        private readonly IAccountService _accountService;
        private readonly ICardService _cardService;

        public PersonController(IPersonService personService, IAccountService accountService, ICardService cardService)
        {
            _personService = personService;
            _accountService = accountService;
            _cardService = cardService;
        }

        [Authorize(policy:"UserOnly")]
        [HttpPost("Cards")]
        public async Task<IActionResult> SeeCards()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Name);
            var (success, message, data) = await _cardService.SeeCardsAsync(userEmail);
            if (!success)
            {
                return BadRequest(message);
            }
            return Ok(new { message, data });
        }
    }
}
