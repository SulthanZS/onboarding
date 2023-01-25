using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NuGet.Protocol.Core.Types;
using OB_Net_Core_Tutorial.DAL.Data;
using OB_Net_Core_Tutorial.DAL.Repository;
using OB_Net_Core_Tutorial.DAL.Models;
using System.Configuration;
using Quartz;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OB_Net_Core_Tutorial.BLL.Service;
using OB_Net_Core_Tutorial.BLL.Interface;
using OB_Net_Core_Tutorial.QuartzProject.Service;
using OB_Net_Core_Tutorial.QuartzProject.Interface;
using OB_Net_Core_Tutorial.QuartzProject.Jobs;
using OB_Net_Core_Tutorial.QuartzProject;
using OBNetCoreTutorial.BLL.Interface;
using OB_Net_Core_Tutorial.BLL.Service;
using OB_Net_Core_Tutorial.DAL.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddRazorPages();
builder.Services.AddDbContext<OB_Net_Core_TutorialContext>(options =>
    options.UseSqlServer("Data Source=sulthan-db-onboarding.database.windows.net;Initial Catalog=SQLOnboarding;User ID=sqladmin;Password=P@ssw0rd"));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<UnitOfWork>();
builder.Services.AddDistributedRedisCache(option =>
{
    option.Configuration = builder.Configuration["AzureRedisCacheConnection"];
});

builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();
});
builder.Services.AddQuartzHostedService(opt =>
{
    opt.WaitForJobsToComplete = true;
});

builder.Services.AddMvc();
builder.Services.AddSingleton<IRedisService, RedisService>();
builder.Services.AddSingleton<IPublisherService, PublisherService>();
builder.Services.AddSingleton<ISchedulerService, SchedulerService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICarService, CarService>();
builder.Services.AddTransient<Job>();
builder.Services.AddTransient<QuartzJobFactory>();
builder.Services.AddApplicationInsightsTelemetry(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]);

var app = builder.Build();

var schedulerService = app.Services.GetRequiredService<ISchedulerService>();
//schedulerService.Initialize("0/10 * * * * ? ");
//schedulerService.Start();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
