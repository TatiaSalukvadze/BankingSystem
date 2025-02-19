using BankingSystem.Application.Services;
using BankingSystem.Contracts;
using BankingSystem.Contracts.DTOs;
using BankingSystem.Contracts.Interfaces;
using BankingSystem.Contracts.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Security.Claims;


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


        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;

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
