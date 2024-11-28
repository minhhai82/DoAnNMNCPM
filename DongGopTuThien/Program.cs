using DongGopTuThien.Entities;
using DongGopTuThien.Services;
using Microsoft.EntityFrameworkCore;

namespace DongGopTuThien
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            // Add services to the container.
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
            builder.Services.AddScoped<IJwtService, JwtService>();

            var twilioConfig = builder.Configuration.GetSection("Twilio").Get<TwilioConfig>();

            builder.Services.AddScoped<IOTPService, OTPService>(provider => new OTPService(twilioConfig.AccountSid, twilioConfig.AuthToken, twilioConfig.ServiceId));

            builder.Services.AddControllers();
            builder.Services.AddSignalR();// Add push notification
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            //khai báo database
            builder.Services.AddDbContext<DaQltuThienContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("QLTuThien")));
            //cho phép truy cập API từ web site khác
            builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseRouting();

            app.UseCors();

            app.UseMiddleware<JwtMiddleware>();

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();
            app.MapHub<NotificationService>("/notificationService"); 

            app.Run();
        }
    }
}
