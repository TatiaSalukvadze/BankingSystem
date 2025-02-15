using Microsoft.AspNetCore.Mvc;

namespace BankingSystem.API.Controllers.Reports
{
    [Route("/Report/[controller]")]
    public class TransactionStatisticsController : WrapperController
    {

        [HttpPost]
        [Route("SeeBalance")]
        public IActionResult Authenticate()
        {

            return Ok();
        }
    }
}
