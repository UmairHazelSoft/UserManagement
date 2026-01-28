namespace UserManagementSystem.Middleware
{
    using System.Net;
    using System.Text.Json;
    using UserManagementSystem.DTOs;

    public class ApiResponseMiddleware
    {
        private readonly RequestDelegate _next;

        public ApiResponseMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);

                if (context.Response.StatusCode == StatusCodes.Status404NotFound &&
                    !context.Response.HasStarted)
                {
                    context.Response.ContentType = "application/json";

                    var response = new ApiResponse<object>(
                        null,
                        false,
                        "Resource not found",
                        HttpStatusCode.NotFound
                    );

                    await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                }
            }
            catch (Exception)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                var response = new ApiResponse<object>(
                    null,
                    false,
                    "An unexpected server error occurred",
                    HttpStatusCode.InternalServerError
                );

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }

}
