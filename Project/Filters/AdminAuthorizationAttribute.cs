using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Project.Filters
{
    /// <summary>
    /// Authorization attribute that ensures only authenticated employees can access admin area
    /// </summary>
    public class AdminAuthorizationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            var userType = session.GetString("UserType");
            var userId = session.GetString("UserId");

            // Check if user is logged in and is an Employee
            if (string.IsNullOrEmpty(userId) || userType != "Employee")
            {
                // Redirect to login page with return URL
                context.Result = new RedirectToActionResult("Login", "Account", new { area = "", returnUrl = context.HttpContext.Request.Path });
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
