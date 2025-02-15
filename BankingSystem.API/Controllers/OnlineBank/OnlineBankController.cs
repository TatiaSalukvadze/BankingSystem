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
        [HttpPost]  
        public async Task<IActionResult> RegisterPerson([FromForm] RegisterDTO registerDto)
        {
            var (success, Message, data) = await _personService.RegisterPersonAsync(registerDto);
            if (!success) {
                return BadRequest(Message);
            }
            return Ok(new {Message, data});
        }
    }
}
