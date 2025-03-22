namespace BankingSystem.Contracts.Response
{
    public class SimpleResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }

        public SimpleResponse Set(bool success, string message, int statusCode = 200) {
            Success = success;
            Message = message;
            StatusCode = statusCode;
            return this;
        }
    }
}
