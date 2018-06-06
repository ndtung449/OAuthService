using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using IdentityModel;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OAuthService.Core;
using OAuthService.Core.Repositories;
using OAuthService.Core.Services;
using OAuthService.Domain.Entities;
using Swashbuckle.AspNetCore.Swagger;

namespace OAuthService.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            string connectionString = Configuration.GetConnectionString("DefaultConnection");

            string migrationsAssembly = typeof(ApplicationDbContext).GetTypeInfo().Assembly.GetName().Name;

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString, b => b.MigrationsAssembly(migrationsAssembly)));

            services.AddIdentity<User, Role>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddIdentityServer()
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                    {
                        builder.UseSqlServer(connectionString,
                            sqlOptions => sqlOptions.MigrationsAssembly(migrationsAssembly));
                    };
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                    {
                        builder.UseSqlServer(connectionString,
                            sqlOptions => sqlOptions.MigrationsAssembly(migrationsAssembly));
                    };
                })
                .AddAspNetIdentity<User>()
                .AddDeveloperSigningCredential();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Auth Service API", Version = "v1" });
            });

            ConfigureDI(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Auth Service Api v1");
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseIdentityServer();
            InitializeDbTestData(app);

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private void ConfigureDI(IServiceCollection services)
        {
            //services.AddSingleton<IEmailSender, EmailSender>();
            //services.AddSingleton<ISmsSender, SmsSender>();
            services.AddScoped<IClientService, ClientService>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped(typeof(IConfigurationRepository<>), typeof(ConfigurationRepository<>));
            services.AddSingleton<ISecretGenerator, SecretGenerator>();
            services.AddScoped<IApiResourceService, ApiResourceService>();
            services.AddScoped<IClientProfileService, ClientProfileService>();
        }

        private static void InitializeDbTestData(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
                scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>().Database.Migrate();
                scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.Migrate();

                var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

                var identityResources = new List<IdentityResource> {
                    new IdentityResources.OpenId(),
                    new IdentityResources.Profile(),
                    new IdentityResources.Email(),
                    new IdentityResource {
                        Name = "role",
                        UserClaims = new List<string> {"role"}
                    }
                };

                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in identityResources)
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiResources.Any())
                {
                    var resource = new ApiResource
                    {
                        Name = "api1",
                        DisplayName = "API 1",
                        Description = "Test API",
                        UserClaims = new List<string> { "role" },
                        ApiSecrets = new List<Secret> { new Secret("scopeSecret".Sha256()) },
                        Scopes = new List<Scope> {
                            new Scope("scope1"),
                            new Scope("scope2")
                        }
                    };

                    context.ApiResources.Add(resource.ToEntity());
                    context.SaveChanges();
                }

                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                if (!userManager.Users.Any())
                {
                    var claims = new List<Claim>
                    {
                        new Claim(JwtClaimTypes.Email, "admin@test.com"),
                        new Claim(JwtClaimTypes.Role, "admin")
                    };

                    var identityUser = new User()
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserName = "admin@test.com",
                        SecurityStamp = Guid.NewGuid().ToString()
                    };

                    userManager.CreateAsync(identityUser, "password").Wait();
                    userManager.AddClaimsAsync(identityUser, claims).Wait();
                }
            }
        }
    }
}
