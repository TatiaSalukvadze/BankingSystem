using BankingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Contracts.DTOs.UserBanking
{
    public class TransferAccountsDTO
    {
        public Account  From { get; set; }
        public Account To { get; set; }
    }
}
