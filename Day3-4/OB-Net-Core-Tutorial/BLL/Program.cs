using OB_Net_Core_Tutorial.DAL.Models;
using OB_Net_Core_Tutorial.DAL.Repositories;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<Car>();
//builder.Services.AddScoped<IUnitOfWork>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
