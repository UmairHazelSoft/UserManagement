using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using UserManagementSystem.DTOs;

namespace UserManagementSystem.Controllers.Base
{
    [ApiController]
    public class BaseController : ControllerBase
    {
        
        public override OkObjectResult Ok(object? value)
        {
            var response = new ApiResponse<object>(
                value,
                true,
                "Success"
            );
            return base.Ok(response);
        }

        public override BadRequestObjectResult BadRequest(object? error)
        {
            var response = new ApiResponse<object>(
                null,
                false,
                error?.ToString() ?? "Bad Request"
            );
            return base.BadRequest(response);
        }

    }
}
