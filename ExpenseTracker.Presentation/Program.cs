using ExpenseTracker.Application.Services;
using ExpenseTracker.Core.Repositories;
using ExpenseTracker.Core.Services;
using ExpenseTracker.Infra.Persistence;
using ExpenseTracker.Infra.Persistence.Repositories;
using ExpenseTracker.Presentation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddHttpClient();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new Exception("Invalid connection string");
builder.Services.AddDbContext<ExpenseTrackerDbContext>(options =>
   

options.UseNpgsql(connectionString)
);

Configuration.GoogleClientId = builder.Configuration.GetValue<string>("Google:ClientId") ?? throw new Exception("Invalid google client id");
Configuration.GoogleClientSecret = builder.Configuration.GetValue<string>("Google:ClientSecret") ?? throw new Exception("Invalid google client secret");

var corsPolicyName = "allow";
builder.Services.AddCors(options =>
{
    options.AddPolicy(corsPolicyName,
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.UseCors(corsPolicyName);

app.Run();


