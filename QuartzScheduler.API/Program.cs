using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Quartz;
using QuartzScheduler.API.Options;
using QuartzScheduler.API.Middleware;
using QuartzScheduler.API.Services;
using QuartzScheduler.API.Validators;
using QuartzScheduler.API.Scheduling;
using QuartzScheduler.Data.Context;
using QuartzScheduler.Data.Repositories;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddQuartz();
builder.Services.AddQuartzHostedService(options =>
{
    options.WaitForJobsToComplete = true;
});

builder.Services.AddHttpClient<IHttpApiService, HttpApiService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IJobHistoryRepository, JobHistoryRepository>();
builder.Services.AddScoped<IJobRepository, JobRepository>();
builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<ISchedulerService,SchedulerService>();

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddValidatorsFromAssemblyContaining<CreateEmailJobRequestValidator>();
// scans the assembly for IRegister implementations and applies them
TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());
TypeAdapterConfig.GlobalSettings.Compile();
var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

// re-register every active job with Quartz on startup
using (var scope = app.Services.CreateScope())
{
    var schedulerService = scope.ServiceProvider.GetRequiredService<ISchedulerService>();
    await schedulerService.RescheduleAllActiveJobsAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
