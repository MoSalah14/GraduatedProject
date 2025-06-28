using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OutbornE_commerce.BAL.EmailServices;
using OutbornE_commerce.BAL.Extentions;
using OutbornE_commerce.BAL.External_Logins;
using OutbornE_commerce.DAL.Data;
using OutbornE_commerce.Extensions;
using StackExchange.Redis;
using Stripe;
using System.Text;

namespace SweetyCake.Extensions
{
    public static class ServicesConfiguration
    {
        public static void ConfigureProjectServices(this IServiceCollection services, IConfiguration Configuration)
        {
            services.Configure<JWT>(Configuration.GetSection("JWT"));
            services.AddCors(options =>
            {
                options.AddPolicy("_myAllowSpecificOrigins", poicy =>
                poicy.AllowAnyMethod()
                .AllowAnyHeader()
                .AllowAnyOrigin());
            });

            services.AddHttpClient();
            services.AddControllersWithViews();
            services.AddHttpContextAccessor();


            services.AddSingleton<IConnectionMultiplexer>(co =>
            {
                var configuration = Configuration.GetConnectionString("RedisServer");
                return ConnectionMultiplexer.Connect(configuration);
            });

            services.AddIdentity<User, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                            .AddEntityFrameworkStores<ApplicationDbContext>()
                            .AddUserManager<UserManager<User>>()
                            .AddRoles<IdentityRole>()
                            .AddDefaultTokenProviders()
                            .AddSignInManager<SignInManager<User>>();

            services.AddDbContext<ApplicationDbContext>(options =>
                            options.UseSqlServer(
                                Configuration.GetConnectionString("DefaultConnection"),
            b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            services.Configure<ExternalLoginAuth>(Configuration.GetSection("ExternalLoginAuth"));
            services.Configure<FrontBaseUrlSettings>(Configuration.GetSection("FrontBaseUrl"));

            services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));
            services.Configure<StripeSettings>(Configuration.GetSection("Stripe"));
            StripeConfiguration.ApiKey = Configuration["Stripe:SecretKey"];

            services.AddAuthentication(options =>
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
                                    ValidIssuer = Configuration["JWT:Issuer"],
                                    ValidAudience = Configuration["JWT:Audience"],
                                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Key"])),
                                    ClockSkew = TimeSpan.Zero,
                                };
                                // Google Auth
                                // Test Deployment
                            }).AddGoogle(options =>
                            {
                                options.ClientId = Configuration["ExternalLoginAuth:Google:ClientId"];
                                options.ClientSecret = Configuration["ExternalLoginAuth:Google:ClientSecret"];

                                // Cookie For Mvc Authrization
                            }).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                            {
                                options.LoginPath = "/Account/Login";
                            })
                            // FaceBook Auth
                            .AddFacebook(options =>
                            {
                                options.AppId = Configuration["ExternalLoginAuth:FaceBook:ClientId"];
                                options.ClientSecret = Configuration["ExternalLoginAuth:FaceBook:ClientId"];
                                options.AccessDeniedPath = "/AccessDeniedPathInfo";
                            });

            services.AddSwaggerGen(c =>
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

            services.ConfigureLifeTime();

            services.AddEndpointsApiExplorer();

        }




    }
}
