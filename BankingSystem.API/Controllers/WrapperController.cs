using Microsoft.AspNetCore.Mvc;

namespace BankingSystem.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WrapperController : ControllerBase
    {
        protected async Task<IActionResult> HandleResult(bool success, string message, object data = null)
        {
            if (!success)
            {
                return BadRequest(message);
            }
            if(data is null)
            {
                return Ok(new { message });
            }

            return Ok(new { message, data });
        }
    }
}
