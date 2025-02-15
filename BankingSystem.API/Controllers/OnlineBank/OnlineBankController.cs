using BankingSystem.Contracts.DTOs;
using BankingSystem.Contracts.Interfaces.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystem.API.Controllers.OnlineBank
{
    public class OnlineBankController : WrapperController
    {
        private readonly IPersonService _personService;

        public OnlineBankController(IPersonService personService)
        {
            _personService = personService;
        }
        [HttpPost("RegisterPerson")]
        public async Task<IActionResult> RegisterPerson([FromForm] RegisterPersonDTO registerDto)
        {
            var (success, message, data) = await _personService.RegisterPersonAsync(registerDto);
            if (!success) {
                return BadRequest(message);
            }
            return Ok(new {message, data});
        }
        [HttpPost("CreateAccount")]
        public async Task<IActionResult> CreateAccount([FromForm] CreateAccountDTO createAccountDto)
        {
            var (success, message, data) = await _personService.CreateAccountAsync(createAccountDto);
            if (!success)
            {
                return BadRequest(message);
            }
            return Ok(new { message, data });
        }
    }
}
