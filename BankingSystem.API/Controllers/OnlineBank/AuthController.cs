using BankingSystem.Contracts.DTOs;
using BankingSystem.Contracts.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystem.API.Controllers.OnlineBank
{
    [Route("OnlineBank")]
    public class AuthController : WrapperController
    {
        private readonly IPersonService _personService;

        public AuthController(IPersonService personService)
        {
            _personService = personService;
        }

        //[HttpPost]
        //[Route("Login")]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromForm] LoginDTO loginDto)
        {
            var (success, message, data) = await _personService.LoginPersonAsync(loginDto);

            if (!success)
            {
                return BadRequest(message);
            }

            return Ok(new { message, data });
        }

    }
}
