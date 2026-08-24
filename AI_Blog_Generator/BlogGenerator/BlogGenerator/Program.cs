using Microsoft.EntityFrameworkCore;
using BlogGenerator.DAL;
using BlogGenerator.BAL;
using BlogGenerator.Foundation.Middlewares;
using Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BlogGenerator.Interfaces;
using QuestPDF.Infrastructure;
using BlogGenerator.Interfaces.Authentication;
using BlogGenerator.BAL.Authentication;
using BlogGenerator.Interfaces.Profile;
using BlogGenerator.BAL.Profile;
using BlogGenerator.BAL.Blog;
using BlogGenerator.Interfaces.Blog;
using BlogGenerator.Services;
using BlogGenerator.BAL.Category;

var builder = WebApplication.CreateBuilder(args);

QuestPDF.Settings.License = LicenseType.Evaluation;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .WriteTo.Console()
    .WriteTo.File(
        "Logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthorization();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();

builder.Services.AddScoped<IProfileService, ProfileService>();

builder.Services.AddScoped<IBlogService, BlogService>();

builder.Services.AddScoped<ICreditService, CreditService>();
builder.Services.AddScoped<IPlanService, PlanService>();

builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IAIBlogService, AIBlogService>();
builder.Services.AddHttpClient<IAIProviderService, AIProviderService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseSerilogRequestLogging();

app.UseGlobalExceptionMiddleware();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
