using BankingSystem.Contracts.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet("Count")]
        public async Task<IActionResult> RegisteredPeopleStatistics()
        {
            var response = await _personService.RegisteredPeopleStatisticsAsync();
            return new ObjectResult(response);
        }
    }
}
