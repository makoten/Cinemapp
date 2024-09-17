using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Movies.Api.Mapping;
using Movies.Application;
using Movies.Application.Database;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

// Add services to the container.

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x => 
{
    x.TokenValidationParameters = new TokenValidationParameters
    {
        // lots of questionable ideas here, but this is demonstrating basic concepts, not production
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!)),
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidIssuer = config["Jwt:Issuer"],
        ValidAudience = config["Jwt:Audience"],
        ValidateIssuer = true,
        ValidateAudience = true
    };
});

builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// this logic should live in the Application layer for decoupling
//builder.Services.AddSingleton<IMovieRepository, MovieRepository>();
// Instead, created an extension method
builder.Services.AddApplication();
builder.Services.AddDatabase(config["Database:ConnectionString"]!);

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ValidationMappingMiddleware>();
app.MapControllers();

var dbInitializer = app.Services.GetRequiredService<DbInitializer>();
await dbInitializer.InitializeAsync();

app.Run();
