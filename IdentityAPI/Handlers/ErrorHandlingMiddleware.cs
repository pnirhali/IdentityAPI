using IdentityAPI.CustomExceptions;
using IdentityAPI.DTO;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace IdentityAPI.Handlers
{
    public class ErrorHandlingMiddleware
    {

        private readonly RequestDelegate Next;
        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            Next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await Next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }
        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var code = HttpStatusCode.InternalServerError;
            if (ex is InvalidPasswordException)
            {
                code = HttpStatusCode.BadRequest;
            }

            Log.Error(ex, "error has been occured");
            var result = JsonConvert.SerializeObject(
                new ErrorDTO
                {
                    Code = (int)code,
                    ErrMessages = ex.Message
                });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }

    }
}
