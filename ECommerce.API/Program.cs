
using DotNetEnv;
using ECommerce.API.Data;
using ECommerce.API.Models;
using ECommerce.API.Services;
using ECommerce.API.Utility;
using ECommerce.API.Utility.DBInitializer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Stripe;
using Stripe.Checkout;
using System.Text;

namespace ECommerce.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            Env.Load();
            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                                  policy =>
                                  {
                                      policy.AllowAnyOrigin()
                                      .AllowAnyMethod()
                                      .AllowAnyHeader();
                                  });
            });
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("defaultConnection")));
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IBrandService, BrandService>();
            builder.Services.AddScoped<IProductService, Services.ProductService>();
            builder.Services.AddScoped<ICartService, CartService>();
            builder.Services.AddTransient<IEmailSender, EmailSender>();
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = false;
                options.SignIn.RequireConfirmedEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()//Database context
            .AddDefaultTokenProviders();
            builder.Services.AddScoped<IDBInitializer, DBInitializer>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IAccountService, Services.AccountService>();
            builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
            StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];
            builder.Services.AddScoped<ICheckOutService, CheckOutService>();
            builder.Services.AddSingleton<SessionService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;//"Use JWT bearer tokens when you need to authenticate a user."
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;//This sets the default scheme to challenge unauthorized users (e.g., when a user tries to access a protected endpoint but isn�t authenticated).
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new()
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,//this will validate the lifetime of the token
                    ValidateIssuerSigningKey = true,//this will validate the signing key of the token
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET")))// this will make sure that the token is signed with the same key that is used to sign the token
                };
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.UseHttpsRedirection();
            app.UseCors(MyAllowSpecificOrigins);
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();
            //this will create a scope for the application and then get the service from the scope then 
            var scope = app.Services.CreateScope();
            var service = scope.ServiceProvider.GetService<IDBInitializer>();
            service.Initialize();
            app.Run();
        }
    }
}
