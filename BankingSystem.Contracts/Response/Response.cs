﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Contracts.Response
{
    public class Response<T> where T : class
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public Response<T> Set(bool success, string message, T data = null) {
            Success = success;
            Message = message;
            Data = data;
            return this;
        }
    }
}
