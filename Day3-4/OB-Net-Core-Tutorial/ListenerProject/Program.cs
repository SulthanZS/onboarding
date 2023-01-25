// See https://aka.ms/new-console-template for more information
using OB_Net_Core_Tutorial.ListenerProject;

Console.WriteLine("Hello, World!");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<MessageListener>();

var app = builder.Build();
app.Run();
