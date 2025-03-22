using Microsoft.AspNetCore.Http;

namespace BankingSystem.Contracts.Response
{
    public class Response<T> where T : class
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public T Data { get; set; }

        public Response<T> Set(bool success, string message, T data = null, int statusCode = 400 ) {
            Success = success;
            Message = message;
            StatusCode = statusCode;
            Data = data;
            return this;
        }
    }
}
