using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json.Serialization;

namespace UserManagementSystem.DTOs
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }     
        public string Message { get; set; }    
        public T Data { get; set; }            

        [JsonIgnore] 
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;

        public ApiResponse() { }

        public ApiResponse(T data, bool success = true, string message = "", HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            Data = data;
            Success = success;
            Message = message;
            StatusCode = statusCode;
        }
    }
}
