
using Serilog;
using GetFlight.Application;
using GetFlight.Infrastructure;
using System.Text.Json.Serialization;
using GetFlight.API.Middleware;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace GetFlight.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // настройка Serilog
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


            // регистрация слоев Application и Infrastructure
            builder.Services.AddApplication();
            builder.Services.AddInfrastructure();

            // настройка LazyCache
            builder.Services.AddLazyCache();

            // Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "GetFlight API",
                    Version = "v1",
                    Description = "API для агрегации данных о перелетах",
                    Contact = new OpenApiContact
                    {
                        Name = "Anton Kuznetsov",
                        Email = "kuzant@gmail.com"
                    }
                });

                // включение комментариев XML для документации API
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                // настройка авторизации в Swagger UI (т.к. используем JWT)
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                     { 
                        new OpenApiSecurityScheme 
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                     }
                });

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
