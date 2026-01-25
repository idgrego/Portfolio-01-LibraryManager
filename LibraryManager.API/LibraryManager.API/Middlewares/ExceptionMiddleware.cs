using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;
using System.Text.Json;

namespace LibraryManager.API.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            this._next = next;
            this._logger = logger;
            this._env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await this._next(context);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, ex.Message);
                await HandlerExceptionAsync(context, ex);
            }
        }

        private async Task HandlerExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            // Criamos um padrão de resposta chamado ProblemDetails (Padrão da Indústria)

            ProblemDetails problem = new ProblemDetails
            {
                Status = _env.IsDevelopment() ? (int)context.Response.StatusCode  : (int)HttpStatusCode.InternalServerError,
                Detail = _env.IsDevelopment() ? ex.ToString() : "Tente novamente mais tarde",
                Title = _env.IsDevelopment() ? "Falhou!" : "Ocorreu um erro interno no servidor. Tente novamente mais tarde.", 
                Instance = context.Request.Path
            };

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(problem, options);

            await context.Response.WriteAsync(json);
        }
    }
}
