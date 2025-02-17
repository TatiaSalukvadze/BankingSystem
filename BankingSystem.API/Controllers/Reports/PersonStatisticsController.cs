using BankingSystem.Application.Services;
using BankingSystem.Contracts.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BankingSystem.API.Controllers.Reports
{
    [Route("/Report/[controller]")]
    public class PersonStatisticsController : WrapperController
    {

        private readonly IPersonService _personService;

        public PersonStatisticsController(IPersonService personService)
        {
            _personService = personService;
        }


        //[Authorize(policy: "ManagerOnly")]
        [HttpGet]
        public async Task<IActionResult> RegisteredPeopleStatistics()
        {

            var (success, message, data) = await _personService.RegisteredPeopleStatisticsAsync();

            if (!success)
            {
                return BadRequest(new { message });
            }

            return Ok(new { message, data });
        }
    }
}
