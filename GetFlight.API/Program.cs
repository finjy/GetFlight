
using Serilog;
using GetFlight.Application;
using GetFlight.Infrastructure;
using System.Text.Json.Serialization;
using GetFlight.API.Middleware;

namespace GetFlight.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Настройка Serilog
            builder.Host.UseSerilog((context, services, configuration) => configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File("logs/getflight-.log", rollingInterval: RollingInterval.Day));

            builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });


            // Регистрация слоев Application и Infrastructure
            builder.Services.AddApplication();
            builder.Services.AddInfrastructure();

            // Настройка LazyCache
            builder.Services.AddLazyCache();

            // Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { Title = "GetFlight API", Version = "v1" });
            });

            // TODO Настройка JWT аутентификации
            // ...

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // Добавление custom middleware
            app.UseMiddleware<RequestLoggingMiddleware>();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
