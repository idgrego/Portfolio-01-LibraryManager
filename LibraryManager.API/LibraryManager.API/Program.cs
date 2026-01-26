#define POSTGRES

using LibraryManager.API.Data;
using LibraryManager.API.Interfaces;
using LibraryManager.API.Middlewares;
using LibraryManager.API.Repositories;
using LibraryManager.API.Services;
using Microsoft.EntityFrameworkCore;
using System;

namespace LibraryManager.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(); // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

            // configurando o acesso ao banco de dados
            // não funciona para async, só para sync
            //builder.Services.AddDbContext<LibraryDbContext>(options =>
            //{
            //    string? conStr = builder.Configuration.GetConnectionString("DefaultConnection");
            //    //options.UseNpgsql(conStr);
            //    options.UseSqlServer(conStr);
            //});

            // essa abordagem funciona para ambas as modalidades: sync e async
            builder.Services.AddDbContextFactory<LibraryDbContext>(options =>
            {
                string? conStr = builder.Configuration.GetConnectionString("DefaultConnection");
#if POSTGRES
                options.UseNpgsql(conStr);
#else
                options.UseSqlServer(conStr);
#endif
            });

            // configurando as injeções de dependências
            builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
            builder.Services.AddScoped<IBookRepository, BookRepository>();
            builder.Services.AddScoped<IAuthorService, AuthorService>();
            builder.Services.AddScoped<IBookService, BookService>();

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    // Isso impede o loop infinito de serialização
                    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                    // Garante que o Angular receba camelCase (id, name) e não PascalCase (Id, Name)
                    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                });

            // configurando o CORS (deve ser antes do builder.Build()
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("DevCors", policy =>
                {
                    policy.AllowAnyOrigin()   // Permite qualquer site (localhost:4200, 3000, etc.)
                          .AllowAnyMethod()   // Permite GET, POST, PUT, DELETE, etc.
                          .AllowAnyHeader();  // Permite qualquer cabeçalho (Content-Type, Authorization...)
                });
            });

            var app = builder.Build();

            // 1. Tratamento de Erros
            app.UseMiddleware<ExceptionMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseCors("DevCors");
            }

            // 2. Redirecionamento HTTPS
            app.UseHttpsRedirection();

            // 3.CORS - Deve vir ANTES de qualquer tentativa de acessar os Controllers
            // app.UseCors("AngularApp");

            // 4. Autorização (se houver)
            app.UseAuthorization();

            // 5. Mapeamento final
            // internamente ele irá invocar o app.UseRouting() e app.UseEndpoints()
            app.MapControllers();


            // --- BLOCO DE AUTOMAÇÃO ---
            //using (var scope = app.Services.CreateScope())
            //{
            //    var services = scope.ServiceProvider;
            //    try
            //    {
            //        var context = services.GetRequiredService<LibraryDbContext>();
            //        // Este comando verifica se o banco existe, cria se necessário 
            //        // e aplica todas as Migrations pendentes.
            //        context.Database.Migrate();
            //    }
            //    catch (Exception ex)
            //    {
            //        var logger = services.GetRequiredService<ILogger<Program>>();
            //        logger.LogError(ex, "Ocorreu um erro ao aplicar as migrações.");
            //    }
            //}

            app.Run();
        }
    }
}
