using Microsoft.AspNetCore.Mvc;

namespace BankingSystem.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WrapperController : ControllerBase
    {
        //protected async Task<IActionResult> HandleResult(bool success, string message, object data = null)
        //{
        //    return new ObjectResult(data)
        //    {
        //        StatusCode = 404//StatusCodes.Status422UnprocessableEntity
        //    };
        //    if (!success)
        //    {
        //        return BadRequest(message);
        //    }
        //    if(data is null)
        //    {
        //        return Ok(new { message });
        //    }

        //    return Ok(new { message, data });
        //}
    }
}
