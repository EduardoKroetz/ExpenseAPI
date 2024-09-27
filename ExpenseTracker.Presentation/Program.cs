using ExpenseTracker.Application.Services;
using ExpenseTracker.Core.Repositories;
using ExpenseTracker.Core.Services;
using ExpenseTracker.Infra.Persistence;
using ExpenseTracker.Infra.Persistence.Repositories;
using ExpenseTracker.Presentation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddHttpClient();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IExpenseRepository, ExpenseRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

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

var jwtKey = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new Exception("Invalid jwt key"));
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey(jwtKey),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(corsPolicyName);
app.UseHttpsRedirection();
app.MapControllers();

app.Run();


