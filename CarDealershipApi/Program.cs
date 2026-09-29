using CarDealershipApi.Data;
using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using System.Text.Json.Serialization;

namespace CarDealershipApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var connString = builder.Configuration.GetConnectionString("Default") ?? "Data Source=dealership.db;Foreign Keys=True";
            builder.Services.AddSingleton(new Data.DbConnectionFactory(connString));
            builder.Services.AddAuthenticationJwtBearer(s => s.SigningKey =
            builder.Configuration["Jwt:SigningKey"] ?? "default_signing_key")
                .AddAuthorization()
                .AddFastEndpoints()
                .SwaggerDocument();

            var app = builder.Build();

            DatabaseInitializer.InitializeDatabase(connString);
            if (app.Environment.IsDevelopment())
            {
                DatabaseSeeder.Seed(connString);
            }

            app.UseAuthentication()
                .UseAuthorization()
                .UseFastEndpoints(c =>
                    c.Serializer.Options.Converters.Add(new JsonStringEnumConverter()))
                .UseSwaggerGen();

            app.Run();
        }
    }
}
