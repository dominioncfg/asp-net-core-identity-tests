using IdentityTests.Data;
using IdentityTests.Infra;
using IdentityTests.Models;
using IdentityTests.Services.Email;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace IdentityTests
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<UsersDBContext>(o =>
            {
                string userCS = _configuration["ConnectionStrings:UsersDb"];
                o.UseSqlServer(userCS);
            });

            services
                .AddIdentity<AppIdentityUser, AppIdentityRole>()
                .AddEntityFrameworkStores<UsersDBContext>()
                .AddDefaultTokenProviders();
            services.Configure<DataProtectionTokenProviderOptions>(opt => opt.TokenLifespan = TimeSpan.FromHours(2));

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = true;

                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);

                options.SignIn.RequireConfirmedEmail = true;
                options.User.RequireUniqueEmail = true;
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("UserOfAnyRolePolicy", policy =>
                {
                    string[] roles = Enum.GetNames(typeof(Roles));
                    policy.RequireRole(roles);
                });

                options.AddPolicy("AdminUserOnlyPolicy", policy =>
                {
                    policy.RequireRole(Roles.Admin.ToString());
                });
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Auth/SignIn";
                options.AccessDeniedPath = "/Auth/AccessDenied";
                options.ExpireTimeSpan = TimeSpan.FromDays(7);
            });

            services.Configure<SmtpOptions>(_configuration.GetSection("Smtp"));
            services.Configure<IdentitySeedConfiguration>(_configuration.GetSection("IdentitySeedData"));

            services.AddTransient<IEmailService, EmailService>();
            services.AddControllersWithViews();

            services.AddScoped<IUserClaimsPrincipalFactory<AppIdentityUser>, AppClaimsPrincipalFactory>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });

            IdentitySeedData.CreateIdentitySeedData(app.ApplicationServices);
        }
    }
}
