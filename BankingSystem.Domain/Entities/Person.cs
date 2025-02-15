using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Domain.Entities
{
    public class Person
    {
        public int Id { get; set; }
        public string IdentityUserId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        public string IDNumber { get; set; }
        public DateTime Birthdate { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
