using System.Text.Json;

namespace LabBookingLib.ApiResponses
{


    public class IAPIResponse
    {
        public bool Success { get; set; }
        public JsonElement Data { get; set; }

        public string? Message { get; set; }



    }
    public class ApiErrorException : Exception
    {
        public int Code { get; }

        public ApiErrorException(int code, string message, Exception? innerException = null)
            : base(message, innerException)
        {
            Code = code;
        }
    }

}