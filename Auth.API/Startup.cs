using System.IO;
using Auth.Domain.Authentication;
using Auth.Domain.Configuration;
using IdentityModel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Auth.Data.Context;
using Auth.Data.Access;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;
using Auth.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Auth.API
{
    public class Startup
    {
        private readonly int _sslPort = 443;

        public Startup(IHostingEnvironment env, IConfiguration configuration)
        {
            TelemetryConfiguration.Active.DisableTelemetry = true;

            if (env.IsDevelopment())
            {
                var launConfiguration = new ConfigurationBuilder()
                                            .SetBasePath(env.ContentRootPath)
                                            .AddJsonFile(Path.Combine("Properties", "launchSettings.json"))
                                            .Build();
                _sslPort = launConfiguration.GetValue<int>("iisSettings::iisExpress::sslPort");
            }


            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string connectionString = Configuration.GetConnectionString("DefaultConnection");

            var domainSettings = new DomainSettings();
            var section = Configuration.GetSection(nameof(DomainSettings));
            section.Bind(domainSettings);
            services.Configure<DomainSettings>(options => section.Bind(options));

            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddMvcCore(options =>
            {
                options.SslPort = _sslPort;
            })
            .AddAuthorization()
            .AddJsonFormatters()
            .AddJsonOptions(options =>
             {
                 options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                 options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
             });


            services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = domainSettings.Auth.Url;
                    options.RequireHttpsMetadata = false;
                    options.ApiName = domainSettings.Api.Name;
                    options.ApiSecret = domainSettings.Api.Secret;
                });

            services.AddCors(options =>
            {
                options.AddPolicy("default", policy =>
                {
                    policy.WithOrigins(domainSettings.Client.Url)
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });


            services.AddAuthorization(options =>
            {
                options.AddPolicy(DomainPolicies.NormalUser,
                     policy => policy.RequireClaim(JwtClaimTypes.Scope, DomainScopes.MvcClientUser));
                options.AddPolicy(DomainPolicies.SuperAdmin, policy => policy.RequireClaim(JwtClaimTypes.Role, DomainRoles.SuperAdmin));
                options.AddPolicy(DomainPolicies.Admin, policy => policy.RequireClaim(JwtClaimTypes.Role, DomainRoles.Admin));
            });

            services.AddDbContext<IdentityContext>(options => options.UseSqlServer(connectionString));
            services.AddTransient<EntityManager>();
            services.AddTransient<IUnitOfWork, SiUnitOfWork>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("default");
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
