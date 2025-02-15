using Microsoft.AspNetCore.Mvc;

namespace BankingSystem.API.Controllers.ATM
{
    [Route("/ATM/[controller]")]
    public class CardController : WrapperController
    {
        [HttpPost]
        [Route("SeeBalance")]
        public IActionResult Authenticate()
        {

            return Ok();
        }
    }
}
