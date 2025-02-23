using BankingSystem.Application.Services;
using BankingSystem.Contracts.DTOs;
using BankingSystem.Contracts.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystem.API.Controllers.OnlineBank
{
    //[Authorize(policy: "OperatorOnly")]
    public class OnlineBankController : WrapperController
    {
        private readonly IPersonService _personService;
        private readonly IAccountService _accountService;
        private readonly ICardService _cardService;
        private readonly IIdentityService _identityService;

        public OnlineBankController(IPersonService personService, IAccountService accountService, 
            ICardService cardService, IIdentityService identityService)
        {
            _personService = personService;
            _accountService = accountService;
            _cardService = cardService;
            _identityService = identityService;
        }

        //tatia
        [HttpPost("RegisterPerson")]
        public async Task<IActionResult> RegisterPerson([FromForm] RegisterPersonDTO registerDto)
        {
            var (success, message, data) = await _identityService.RegisterPersonAsync(registerDto);
            if (!success) {
                return BadRequest(message);
            }
            var (finalSuccess, finalMessage, finalData) = await _personService.RegisterCustomPersonAsync(registerDto, data);
            if (!finalSuccess)
            {
                return BadRequest(finalMessage);
            }
            return Ok(new { finalMessage, finalData });
        }
        //tatia
        [HttpPost("CreateAccount")]
        public async Task<IActionResult> CreateAccount([FromForm] CreateAccountDTO createAccountDto)
        {
            var (success, message, data) = await _accountService.CreateAccountAsync(createAccountDto);
            if (!success)
            {
                return BadRequest(message);
            }
            return Ok(new { message, data });
        }
        //tamar
        [HttpPost("CreateCard")]
        public async Task<IActionResult> CreateCard([FromForm] CreateCardDTO createCardDto)
        {
            var (success, message, data) = await _cardService.CreateCardAsync(createCardDto);
            if (!success)
            {
                return BadRequest(message);
            }
            return Ok(new { message, data });
        }

        [HttpDelete("Card")]
        public async Task<IActionResult> CancelCard([FromForm] string cardNumber)
        {
            var (success, message) = await _cardService.CalcelCardAsync(cardNumber);
            if (!success)
            {
                return BadRequest(message);
            }
            return Ok(new { message });
        }
    }
}
