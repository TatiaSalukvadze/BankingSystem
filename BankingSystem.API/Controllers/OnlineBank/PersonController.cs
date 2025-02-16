using BankingSystem.Contracts.DTOs;
using BankingSystem.Contracts.Interfaces.IServices;
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


        [HttpPost("Cards")]
        public async Task<IActionResult> SeeCards([FromForm] RegisterPersonDTO registerDto)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Name);
            var (success, message, data) = await _personService.RegisterPersonAsync(registerDto);
            if (!success)
            {
                return BadRequest(message);
            }
            return Ok(new { message, data });
        }
    }
}
