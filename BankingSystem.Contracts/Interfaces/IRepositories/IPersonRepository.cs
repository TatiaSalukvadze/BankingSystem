using BankingSystem.Domain.Entities;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Contracts.Interfaces.IRepositories
{
    public interface IPersonRepository : IRepository
    {
  
        Task<int> RegisterPersonAsync(Person person);
    }
}
