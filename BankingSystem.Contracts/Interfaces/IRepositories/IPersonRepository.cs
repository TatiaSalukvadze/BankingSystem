﻿using BankingSystem.Domain.Entities;

namespace BankingSystem.Contracts.Interfaces.IRepositories
{
    public interface IPersonRepository : IRepository
    {
        Task<Person?> FindByIdentityIdAsync(string identityId);
        Task<int> FindIdByIDNumberAsync(string IDNumber);
        Task<int> RegisterPersonAsync(Person person);
        Task<int> PeopleRegisteredThisYearAsync();
        Task<int> PeopleRegisteredLastOneYearAsync();
        Task<int> PeopleRegisteredLast30DaysAsync();
    }
}
