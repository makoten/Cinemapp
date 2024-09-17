using FluentValidation;
using Movies.Api.Mapping;
using Movies.Application;
using Movies.Application.Database;
using Movies.Application.Models;
using Movies.Application.Validators;


var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

// Add services to the container.

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

app.UseAuthorization();

app.UseMiddleware<ValidationMappingMiddleware>();
app.MapControllers();

var dbInitializer = app.Services.GetRequiredService<DbInitializer>();
await dbInitializer.InitializeAsync();

app.Run();
