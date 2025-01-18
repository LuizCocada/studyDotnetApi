using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using StudyAPI.Context;
using StudyAPI.Extensions;
using StudyAPI.Filters;
using StudyAPI.Repositories;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers(options =>
{
    options.Filters.Add(typeof(ApiExceptionFilter)); //filtro de exceção
}).AddJsonOptions(options => { options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; });

builder.Services.AddAplication(); //injeção de dependências

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string? mySqlConnection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(mySqlConnection,
        ServerVersion.AutoDetect(mySqlConnection)));

builder.Services.AddScoped<ApiLoggingFilter>(); //filtro de log

var app = builder.Build();

//desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.ConfigureExceptionHandler(); //middlawre to global exception handling in development
}

app.UseHttpsRedirection(); //middleware to redirect HTTP request to HTTPS

app.UseAuthorization(); //middleware to handle authorization

app.MapControllers(); //middleware to map controllers routes

app.Run();