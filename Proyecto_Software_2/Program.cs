
namespace Proyecto_Software_2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

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
            builder.Services.AddSingleton<AppLogic.SeguridadAdmin.SeguridadAdmin>();

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

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();

            app.UseCors("MyPolicy");

            app.MapControllers();

            app.Run();
        }
    }
}
