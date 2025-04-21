using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.S3;
using AppLogic.ConnectorsAdmin;
using Microsoft.AspNetCore.Builder;

namespace Proyecto_Software_2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var awsOptions = builder.Configuration.GetAWSOptions();

            // Add AWS AppConfig configuration
            awsOptions.Credentials = new BasicAWSCredentials(
                builder.Configuration["AWS:AccessKey"],
                builder.Configuration["AWS:SecretKey"]);
            builder.Services.AddDefaultAWSOptions(awsOptions);
            builder.Services.AddAWSService<IAmazonS3>();

            // Add services to the container.
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Registrar AWS S3 Connector
            builder.Services.AddSingleton<AwsConnector>();

            // Registrar Notificador
            builder.Services.AddSingleton<AppLogic.SeguridadAdmin.Notificador>(sp =>
                new AppLogic.SeguridadAdmin.Notificador(
                    smtpServer: builder.Configuration["EmailSettings:SmtpServer"] ?? "smtp.gmail.com",
                    smtpPort: int.Parse(builder.Configuration["EmailSettings:SmtpPort"] ?? "587"),
                    fromEmail: builder.Configuration["EmailSettings:FromEmail"] ?? "yourapplication@example.com",
                    username: builder.Configuration["EmailSettings:Username"] ?? "",
                    password: builder.Configuration["EmailSettings:Password"] ?? "",
                    useSsl: bool.Parse(builder.Configuration["EmailSettings:UseSsl"] ?? "true")
                )
            );

            // Registrar SeguridadAdmin
            builder.Services.AddSingleton<AppLogic.SeguridadAdmin.SeguridadAdministrador>();

            //CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: "MyPolicy",

                    policy =>
                    {
                        policy.AllowAnyHeader();
                        policy.AllowAnyMethod();
                        policy.AllowAnyOrigin();
                    });
                    
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowUI", policy =>
                {
                    policy.WithOrigins("https://proyecto-software-2-ui-drdzbrd3cjgugpap.canadacentral-01.azurewebsites.net")
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowAnyOrigin();
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                // Para usar swagger en el ambiente de prod
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                    options.RoutePrefix = string.Empty;
                });
            }

            app.UseCors("MyPolicy");
            app.UseCors("AllowUI");

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
