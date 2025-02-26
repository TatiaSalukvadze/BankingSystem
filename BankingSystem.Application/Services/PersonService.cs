using BankingSystem.Contracts.DTOs;
using BankingSystem.Contracts.Interfaces;
using BankingSystem.Contracts.Interfaces.IExternalServices;
using BankingSystem.Contracts.Interfaces.IServices;
using BankingSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Application.Services
{
    public class PersonService : IPersonService
    {
        private readonly IUnitOfWork _unitOfWork;


        public PersonService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }

        
        //tatia
        public async Task<(bool Success, string Message, object? Data)> RegisterCustomPersonAsync(RegisterPersonDTO registerDto, string IdentityUserId)
        {

            
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
                return (false, "Adding user in physical person system failed!", null);
            }
            _unitOfWork.SaveChanges();
           
            return (true, "User was registered successfully!",new { IdentityUserId, CustomUserId = personId });
            

        }
        //both
        public async Task<(bool Success, string Message, Dictionary<string, int> statistics)> RegisteredPeopleStatisticsAsync()
        {

            var personStatistics = new Dictionary<string, int>();
            int peopleRegisteredThisYear = await _unitOfWork.PersonRepository.PeopleRegisteredThisYear();
            personStatistics.Add("People Registered This Year", peopleRegisteredThisYear);

            int peopleRegisteredLast1Year = await _unitOfWork.PersonRepository.PeopleRegisteredLastOneYear();
            personStatistics.Add("People Registered Last 1 Year", peopleRegisteredLast1Year);

            int peopleRegisteredLast30Days = await _unitOfWork.PersonRepository.PeopleRegisteredLast30Days();
            personStatistics.Add("People Registered Last 30 Days", peopleRegisteredLast30Days);

            return (true, "Statistics are retrieved!", personStatistics);

        }
    }
}
