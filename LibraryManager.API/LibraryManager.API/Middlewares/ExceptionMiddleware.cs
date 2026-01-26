using LibraryManager.API.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
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
            var statusCode = HttpStatusCode.InternalServerError;
            var message = "Ocorreu um erro inesperado no servidor.";
            

            // 1. Verifica se é uma exceção customizada da nossa aplicação
            if (ex is BaseException appEx)
            {
                statusCode = appEx.StatusCode;
                message = appEx.Message;
            }
            // 2. Mantém a lógica do PostgreSQL que fizemos antes
            else if (ex is DbUpdateException dbEx && dbEx.InnerException is PostgresException pgEx)
            {
                if (pgEx.SqlState == "23505")
                {
                    statusCode = HttpStatusCode.Conflict;
                    message = $"({pgEx.ConstraintName}) {TraduzirMensagemDeErro(pgEx.ConstraintName)}";
                }
            }

            // Criamos um padrão de resposta chamado ProblemDetails (Padrão da Indústria)

            ProblemDetails problem = new ProblemDetails
            {
                Status = _env.IsDevelopment() ? (int)statusCode : (int)HttpStatusCode.InternalServerError,
                Title = _env.IsDevelopment() ? "Falhou!" : "Ocorreu um erro interno no servidor. Tente novamente mais tarde.", 
                Detail = $"{message}{Environment.NewLine}{(_env.IsDevelopment() ? ex.ToString() : "Tente novamente mais tarde")}",
                Instance = context.Request.Path
            };

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(problem, options);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;
            await context.Response.WriteAsync(json);
        }

        private static string TraduzirMensagemDeErro(string? constraintName)
        {
            return constraintName switch
            {
                "IX_Authors_Name" => "Já existe um autor cadastrado com este nome.",
                "IX_Book_ISBN_Unique" => "Este ISBN já está cadastrado em outro livro.",
                "IX_Book_Title_AuthorId_Unique" => "Este autor já possui um livro cadastrado com este título.",
                _ => "Não foi possível salvar os dados devido a uma duplicidade no sistema."
            };
        }
    }
}
