using BankingSystem.Contracts.DTOs.OnlineBank;
using BankingSystem.Contracts.Interfaces;
using BankingSystem.Contracts.Interfaces.IServices;
using BankingSystem.Contracts.Response;
using BankingSystem.Domain.Entities;

namespace BankingSystem.Application.Services
{
    public class PersonService : IPersonService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PersonService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Response<object>> RegisterCustomPersonAsync(RegisterPersonDTO registerDto, string IdentityUserId)
        {
            var response = new Response<object>();
            var person = new Person
            {
                IdentityUserId = IdentityUserId,
                Name = registerDto.Name,
                Surname = registerDto.Surname,
                IDNumber = registerDto.IDNumber,
                Birthdate = registerDto.Birthdate,
                Email = registerDto.Email,
                CreatedAt = DateTime.Now
            };

            var personId = await _unitOfWork.PersonRepository.RegisterPersonAsync(person);
            if (personId <= 0)
            {
                return response.Set(false, "Adding user in physical person system failed!", null, 400);
            }

            return response.Set(true, "User was registered successfully!", new { IdentityUserId, CustomUserId = personId }, 201);
        }

        public async Task<Response<Dictionary<string, int>>> RegisteredPeopleStatisticsAsync()
        {
            var response = new Response<Dictionary<string, int>>();
            Task<int> PeopleRegisteredThisYear = _unitOfWork.PersonRepository.PeopleRegisteredThisYear();
            Task<int> PeopleRegisteredLast1Year = _unitOfWork.PersonRepository.PeopleRegisteredLastOneYear();
            Task<int> PeopleRegisteredLast30Days = _unitOfWork.PersonRepository.PeopleRegisteredLast30Days();

            var result = await Task.WhenAll(PeopleRegisteredThisYear, PeopleRegisteredLast1Year, PeopleRegisteredLast30Days);
            if (result is null || result.Count() != 3)
            {
                return response.Set(false, "Person Statistics couldn't be retrieved!", null, 404);
            }

            var personStatistics = new Dictionary<string, int>()
            {
                { "People Registered This Year", result[0] },
                { "People Registered Last 1 Year", result[1] },
                { "People Registered Last 30 Days", result[2] }
            };

            return response.Set(true, "Statistics are retrieved!", personStatistics, 200);
        }
    }
}
