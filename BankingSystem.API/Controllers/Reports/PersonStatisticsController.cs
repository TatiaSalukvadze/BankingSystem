using BankingSystem.Application.Services;
using BankingSystem.Contracts.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BankingSystem.API.Controllers.Reports
{
    [Authorize(policy: "ManagerOnly")]
    [Route("/Report/[controller]")]
    public class PersonStatisticsController : WrapperController
    {

        private readonly IPersonService _personService;

        public PersonStatisticsController(IPersonService personService)
        {
            _personService = personService;
        }


        [HttpGet]
        public async Task<IActionResult> RegisteredPeopleStatistics()
        {

            var (success, message, data) = await _personService.RegisteredPeopleStatisticsAsync();
            return await HandleResult(success, message, data);
            //if (!success)
            //{
            //    return BadRequest(new { message });
            //}

            //return Ok(new { message, data });
        }
    }
}
