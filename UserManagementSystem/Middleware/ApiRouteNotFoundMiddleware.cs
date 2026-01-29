namespace UserManagementSystem.Middleware
{
    using System.Net;
    using System.Text.Json;
    using UserManagementSystem.DTOs;
    using UserManagementSystem.Helpers;

    public class ApiRouteNotFoundMiddleware
    {
        private readonly RequestDelegate _next;

        public ApiRouteNotFoundMiddleware(RequestDelegate next)
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
                        AppSettings.ResourceNotFound
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
                    AppSettings.ErrorOccurred
                );

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }

}
