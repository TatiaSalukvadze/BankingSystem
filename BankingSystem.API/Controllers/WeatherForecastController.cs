using BankingSystem.Contracts.Interfaces.IExternalServices;
using Microsoft.AspNetCore.Mvc;


namespace BankingSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        private readonly IEmailService _emailService;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IEmailService emailService)
        {
            _logger = logger;
            _emailService = emailService;   
        }

        [HttpPost("SendEmail")]
        public async Task<IActionResult> SendEmailToUser(string email, string subject, string message)
        {
            try
            {
                await _emailService.SendEmailPlaint(email, subject, message);
                return Ok("Email sent successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to send email: {ex.Message}");
            }
        }

        [HttpGet("Accounts")]
        public async Task<IActionResult> SeeAccounts()
        {
            throw new Exception("ldjcm");
            string message = "ksdjcn";
            return BadRequest(new { message });
            //string email = User.Identity.Name; 
            //var userEmail = User.FindFirstValue(ClaimTypes.Name);
            //var (success, message, data) = await _accountService.SeeAccountsAsync(userEmail);

            //if (!success)
            //{
            //    return BadRequest(new { message });
            //}

            //return Ok(new { message, data });
        }
        //[HttpGet(Name = "GetWeatherForecast")]
        ////[Authorize]
        //public IEnumerable<WeatherForecast> Get()
        //{
        //    //throw new Exception("ajsk")

        //    return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        //    {
        //        Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
        //        TemperatureC = Random.Shared.Next(-20, 55),
        //        Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        //    })
        //    .ToArray();
        //}
    }
}
