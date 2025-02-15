using Microsoft.AspNetCore.Mvc;

namespace BankingSystem.API.Controllers.OnlineBank
{
    [Route("OnlineBank")]
    public class AuthController : WrapperController
    {
        [HttpPost]
        [Route("Login")]
        public IActionResult Authenticate()
        {

            return Ok();
        }
    }
}
