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
                return response.Set(false, "Adding user in physical person system failed!");
            }
            //_unitOfWork.SaveChanges();
            return response.Set(true, "User was registered successfully!",new { IdentityUserId, CustomUserId = personId });
        }

        public async Task<Response<Dictionary<string, int>>> RegisteredPeopleStatisticsAsync()
        {
            var response = new Response<Dictionary<string, int>>();
            Task<int> PeopleRegisteredThisYear = _unitOfWork.PersonRepository.PeopleRegisteredThisYear();
            Task<int> PeopleRegisteredLast1Year = _unitOfWork.PersonRepository.PeopleRegisteredLastOneYear();
            Task<int> PeopleRegisteredLast30Days = _unitOfWork.PersonRepository.PeopleRegisteredLast30Days();

            var result = await Task.WhenAll(PeopleRegisteredThisYear, PeopleRegisteredLast1Year, PeopleRegisteredLast30Days);
            //(int peopleRegisteredThisYear, int peopleRegisteredLast1Year, int peopleRegisteredLast30Days)
            //    = (result[0], result[1], result[2]);
            if (result is null || result.Count() != 3)
            {
                return response.Set(false, "Person Statistics couldn't be retrieved!");
            }

            var personStatistics = new Dictionary<string, int>()
            {
                { "People Registered This Year", result[0] },
                { "People Registered Last 1 Year", result[1] },
                { "People Registered Last 30 Days", result[2] }
            };
            //personStatistics.Add("People Registered This Year", result[0]);
            //personStatistics.Add("People Registered Last 1 Year", result[1]);
            //personStatistics.Add("People Registered Last 30 Days", result[2]);

            //int peopleRegisteredThisYear = await _unitOfWork.PersonRepository.PeopleRegisteredThisYear();

            //int peopleRegisteredLast1Year = await _unitOfWork.PersonRepository.PeopleRegisteredLastOneYear();
            //personStatistics.Add("People Registered Last 1 Year", peopleRegisteredLast1Year);

            //int peopleRegisteredLast30Days = await _unitOfWork.PersonRepository.PeopleRegisteredLast30Days();
            //personStatistics.Add("People Registered Last 30 Days", peopleRegisteredLast30Days);

            return response.Set(true, "Statistics are retrieved!", personStatistics);
        }
    }
}
