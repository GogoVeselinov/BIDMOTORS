using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Project.Middleware
{
    /// <summary>
    /// Middleware that blocks unauthorized access to admin area
    /// </summary>
    public class AdminAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public AdminAuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower() ?? "";

            // Check if accessing admin area
            if (path.StartsWith("/admin") || path.StartsWith("/api/admin"))
            {
                var userType = context.Session.GetString("UserType");
                var userId = context.Session.GetString("UserId");

                // If not logged in as employee, redirect to login
                if (string.IsNullOrEmpty(userId) || userType != "Employee")
                {
                    context.Response.Redirect("/Account/Login?returnUrl=" + context.Request.Path);
                    return;
                }
            }

            await _next(context);
        }
    }
}
