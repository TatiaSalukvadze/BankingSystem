using Microsoft.AspNetCore.Mvc;

namespace BankingSystem.API.Controllers.OnlineBank
{
    [Route("/OnlineBank/[controller]")]
    public class PersonController : WrapperController
    {
        [HttpPost]
        [Route("authenticate")]
        public IActionResult Authenticate()
        {

            return Ok();
        }
    }
}
