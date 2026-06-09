using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using AmisPayroll.Application.Exceptions;
using Microsoft.AspNetCore.Http;

namespace AmisPayroll.Api.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = new
            {
                Success = false,
                DevMessage = exception.Message,
                UserMessage = "Có lỗi xảy ra, vui lòng liên hệ bộ phận hỗ trợ.",
                TraceId = context.TraceIdentifier
            };

            switch (exception)
            {
                case ValidateException validateEx:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response = new
                    {
                        Success = false,
                        DevMessage = validateEx.Message,
                        UserMessage = validateEx.Message, 
                        TraceId = context.TraceIdentifier
                    };
                    break;

                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}