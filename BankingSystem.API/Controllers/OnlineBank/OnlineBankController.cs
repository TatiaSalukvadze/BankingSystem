using BankingSystem.Contracts.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystem.API.Controllers.OnlineBank
{
    public class OnlineBankController : WrapperController
    {
        [HttpPost]  
        public async Task<IActionResult> RegisterPerson([FromForm] RegisterDTO registerDto)
        {
            //var (success, Message, data) = await _personService.RegisterPersonAsync(registerDto);
            return Ok();
        }
    }
}
