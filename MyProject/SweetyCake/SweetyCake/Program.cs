using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OutbornE_commerce.BAL.AuthServices;
using OutbornE_commerce.BAL.Dto;
using OutbornE_commerce.BAL.EmailServices;
using OutbornE_commerce.BAL.Extentions;
using OutbornE_commerce.BAL.External_Logins;
using OutbornE_commerce.DAL.Data;
using OutbornE_commerce.DAL.Models;
using OutbornE_commerce.Extensions;
using OutbornE_commerce.FilesManager;
using Serilog;
using StackExchange.Redis;
using Stripe;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog from appsettings.json
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog(); // Use Serilog for logging

// Add services to the container.
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//	.AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));
//string[] CorsOrigins;
//CorsOrigins = builder.Configuration["Cors_Origins"].Split(",");

builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));
builder.Services.AddCors(options =>
{
    options.AddPolicy("_myAllowSpecificOrigins", poicy =>
    poicy.AllowAnyMethod()
    .AllowAnyHeader()
    .AllowAnyOrigin());
});

builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();


builder.Services.AddSingleton<IConnectionMultiplexer>(co =>
{
    var configuration = builder.Configuration.GetConnectionString("RedisServer");
    return ConnectionMultiplexer.Connect(configuration);
});

builder.Services.AddIdentity<User, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddUserManager<UserManager<User>>()
                .AddRoles<IdentityRole>()
                .AddDefaultTokenProviders()
                .AddSignInManager<SignInManager<User>>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

builder.Services.Configure<ExternalLoginAuth>(builder.Configuration.GetSection("ExternalLoginAuth"));
builder.Services.Configure<FrontBaseUrlSettings>(builder.Configuration.GetSection("FrontBaseUrl"));

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
                .AddJwtBearer(o =>
                {
                    o.RequireHttpsMetadata = false;
                    o.SaveToken = false;
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidIssuer = builder.Configuration["JWT:Issuer"],
                        ValidAudience = builder.Configuration["JWT:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),
                        ClockSkew = TimeSpan.Zero
                    };
                    // Google Auth
                    // Test Deployment
                }).AddGoogle(options =>
                {
                    options.ClientId = builder.Configuration["ExternalLoginAuth:Google:ClientId"];
                    options.ClientSecret = builder.Configuration["ExternalLoginAuth:Google:ClientSecret"];
                })
                // FaceBook Auth
                .AddFacebook(options =>
                {
                    options.AppId = builder.Configuration["ExternalLoginAuth:FaceBook:ClientId"];
                    options.ClientSecret = builder.Configuration["ExternalLoginAuth:FaceBook:ClientId"];
                    options.AccessDeniedPath = "/AccessDeniedPathInfo";
                });

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by your JWT token",
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.ConfigureLifeTime();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseDeveloperExceptionPage();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        var contextFeature = context.Features.Get<IExceptionHandlerFeature>();

        if (contextFeature != null)
        {
            // log in both Development and Production
            Log.Error(contextFeature.Error, "An unhandled exception occurred.");

            //if (app.Environment.IsDevelopment())
            //{
            // If in Development, return detailed error information
            await context.Response.WriteAsJsonAsync(new
            {
                StatusCode = context.Response.StatusCode,
                Message = "Internal Server Error. This is a development environment. Here are the details:",
                Detailed = contextFeature.Error,
            });
            //}
            //else
            //{
            //    // In Production, hide details but return a generic message
            //    await context.Response.WriteAsJsonAsync(new
            //    {
            //        StatusCode = context.Response.StatusCode,
            //        Message = "Internal Server Error. Please check the log file for more details."
            //    });
            //}
        }
    });
});

app.UseHttpsRedirection();
//}
app.UseCors("_myAllowSpecificOrigins");
app.UseStaticFiles();

//app.UseJwtExpirationHandling();

// End Localization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

try
{
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}