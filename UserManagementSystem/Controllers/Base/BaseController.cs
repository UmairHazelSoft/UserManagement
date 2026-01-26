using Microsoft.AspNetCore.Mvc;
using UserManagementSystem.DTOs;

namespace UserManagementSystem.Controllers.Base
{
    [ApiController]
    public class BaseController : ControllerBase
    {
        /// <summary>
        /// Converts any ApiResponse<T> to IActionResult automatically
        /// </summary>
        protected IActionResult HandleResponse<T>(ApiResponse<T> response)
        {
            // Always use StatusCode from ApiResponse
            return StatusCode((int)response.StatusCode, response);
        }
    }
}
