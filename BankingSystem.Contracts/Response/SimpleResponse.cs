using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Contracts.Response
{
    public class SimpleResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public SimpleResponse Set(bool success, string message) {
            Success = success;
            Message = message;
            return this;
        }
    }
}
