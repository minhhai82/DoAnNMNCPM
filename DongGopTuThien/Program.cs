using DongGopTuThien.Entities;
using Microsoft.EntityFrameworkCore;

// firebase
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

namespace DongGopTuThien
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile("./credentials/daqltv2024-firebase.json"),
            });

            // Add services to the container.

            builder.Services.AddControllers();
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

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
