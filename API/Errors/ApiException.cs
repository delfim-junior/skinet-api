using System.Net;

namespace API.Errors
{
    public class ApiException : ApiResponse
    {
        public ApiException(int statusCode, string message = null, string details = null) : base((int)statusCode, message)
        {
            Details = details;
        }

        public string Details { get; set; }
    }
}