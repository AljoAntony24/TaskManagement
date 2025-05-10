using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace TaskManagement.Services
{
    public class JWTMiddleware
    {
        private readonly RequestDelegate _next;

        public JWTMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext httpContext,JwtHandler _jwtHandler)
        {
            PathString path = httpContext.Request.Path;
            string token = String.Empty;

            var header = httpContext.Request.Headers["Authorization"];
            if (header.Count == 0)
            {
                if (path == "/api/Common/Login" )
                {
                    await _next(httpContext);
                }
                else { httpContext.Response.StatusCode = 401; }
            }
            else
            {
                string[] tokenValue = Convert.ToString(header).Trim().Split(" ");
                if (tokenValue.Length > 1) token = tokenValue[1];
                if (_jwtHandler.ValidateToken(token))
                {
                    await _next(httpContext);
                }
                else
                {
                    httpContext.Response.StatusCode = 401;
                }
            }
        }

    }
    public static class CustomLogicMiddlewareExtensions
    {
        public static IApplicationBuilder UseJWTMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<JWTMiddleware>();
        }
    }
}
