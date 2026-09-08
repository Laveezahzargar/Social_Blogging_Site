using BlogGenerator.BAL;
using BlogGenerator.BAL.Authentication;
using BlogGenerator.BAL.Blog;
using BlogGenerator.BAL.BlogInteraction;
using BlogGenerator.BAL.Category;
using BlogGenerator.BAL.Feedback;
using BlogGenerator.BAL.Issue;
using BlogGenerator.BAL.Notifications;
using BlogGenerator.BAL.Profile;
using BlogGenerator.DAL;
using BlogGenerator.Foundation.Middlewares;
using BlogGenerator.Interfaces;
using BlogGenerator.Interfaces.Authentication;
using BlogGenerator.Interfaces.Blog;
using BlogGenerator.Interfaces.Profile;
using BlogGenerator.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QuestPDF.Infrastructure;
using Serilog;
using System.Text;

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

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter your JWT token only."
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

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

builder.Services.AddScoped<IImageStorageService,CloudinaryImageStorageService>();
builder.Services.AddScoped<IPublicFeedService, PublicFeedService>();

builder.Services.AddScoped<IBlogInteractionService, BlogInteractionService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
builder.Services.AddScoped<IIssueService, IssueService>();

builder.Services.AddScoped<IPaymentService, PaymentService>();

builder.Services.AddScoped<IAdminService, AdminService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider
        .GetRequiredService<ApplicationDbContext>();

    await DbSeeder.SeedAsync(db);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapGet("/status", () => "Social Blog Site is running");

app.MapGet("/db-status", async (ApplicationDbContext db) =>
{
    try
    {
        var canConnect = await db.Database.CanConnectAsync();

        return Results.Ok(new
        {
            DatabaseConnected = canConnect
        });
    }
    catch (Exception ex)
    {
        return Results.Problem(detail: ex.Message);
    }
});

//app.UseHttpsRedirection();

app.Use(async (context, next) =>
{
    Console.WriteLine($"METHOD: {context.Request.Method}");
    Console.WriteLine($"PATH: {context.Request.Path}");
    Console.WriteLine($"ORIGIN: {context.Request.Headers["Origin"]}");

    await next();
});

app.UseCors("AllowFrontend");

app.UseSerilogRequestLogging();

app.UseGlobalExceptionMiddleware();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
