using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using WebApiGintec.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication
               (JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(options =>
               {
                   options.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateIssuer = true,
                       ValidateAudience = true,
                       ValidateLifetime = true,
                       ValidateIssuerSigningKey = true,

                       ValidIssuer = "etec",
                       ValidAudience = "etec",
                       IssuerSigningKey = new SymmetricSecurityKey
                     (Encoding.UTF8.GetBytes("c013239a-5e89-4749-b0bb-07fe4d21710d"))
                   };
               });
builder.Services.AddAuthorization();

builder.Services.AddDbContext<GintecContext>(options => options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking).UseMySQL("Server=18.208.167.95;Port=3306;Database=Gintec;User=nathan;Password=n@26jlcscrb5c0dB0l;"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();

app.UseAuthorization();

app.UseCors(
             options => options.WithOrigins("*").AllowAnyMethod().AllowAnyHeader()
         );
app.MapControllers();

app.Run();
