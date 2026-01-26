using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using UserManagementSystem.DTOs;

namespace UserManagementSystem.Filters
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Check if model state is invalid
            if (!context.ModelState.IsValid)
            {
                // Combine all errors into a single string
                var errorMessages = context.ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .SelectMany(kvp => kvp.Value.Errors.Select(e => $"{kvp.Key}: {e.ErrorMessage}"))
                    .ToList();

                var combinedMessage = string.Join("; ", errorMessages);

                var response = new ApiResponse<object>(
                    data: null,
                    success: false,
                    message: combinedMessage
                );

                context.Result = new BadRequestObjectResult(response);
            }
        }
    }
}
