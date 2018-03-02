using System.IO;
using System.Reflection;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Auth.Data.Context;
using Auth.Domain.Configuration;
using Auth.Domain.Entities.Identity;
using Auth.Data;
using Auth.Server.Configuration;
using Auth.Server.DatabaseSeed;
using Auth.Server.Certificates;
using Auth.Server.Services;
using Auth.Domain.Authentication;
using IdentityServer4.Validation;
using Auth.server.Configuration;
using IdentityModel;
using Auth.Data.Access;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace Auth.Server
{
    public class Startup
    {
        private readonly IHostingEnvironment _env;
        public IConfiguration Configuration { get; set; }
        private readonly int _sslPort = 443;
        public Startup(IHostingEnvironment env, IConfiguration configuration)
        {
            Configuration = configuration;
            _env = env;

            TelemetryConfiguration.Active.DisableTelemetry = true;

            if (env.IsDevelopment())
            {
                var launchConfiguration = new ConfigurationBuilder()
                    .SetBasePath(env.ContentRootPath)
                    .AddJsonFile(Path.Combine("Properties", "launchSettings.json"))
                    .Build();
                // During development we won't be using port 443
                _sslPort = launchConfiguration.GetValue<int>("iisSettings::iisExpress:sslPort");
            }
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var domainSettings = new DomainSettings();
            Configuration.GetSection(nameof(DomainSettings)).Bind(domainSettings);
            services.Configure<DomainSettings>(options => Configuration.GetSection(nameof(DomainSettings)).Bind(options));

            var appSettings = new AppSettings();
            Configuration.GetSection(nameof(AppSettings)).Bind(appSettings);

            var connectionString = appSettings.ConnectionStrings.AuthContext;
            var migrationsAssembly = typeof(DataModule).GetTypeInfo().Assembly.GetName().Name;

            services.AddDbContext<IdentityContext>(o => o.UseSqlServer(connectionString,
                optionsBuilder =>
                    optionsBuilder.MigrationsAssembly(typeof(DataModule).GetTypeInfo().Assembly.GetName().Name)));
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequiredLength = 6;
                })
                .AddEntityFrameworkStores<IdentityContext>()
                .AddDefaultTokenProviders();

            services.AddMvc(options =>
            {
                //options.Filters.Add(new RequireHttpsAttribute());
                options.SslPort = _sslPort;
            });
            
            // Add application services.
            //services.AddTransient<IResourceOwnerPasswordValidator, ResourceOwnerPasswordValidator>();
            services.AddTransient<IEmailSender, EmailSender>();
            //services.AddTransient<IProfileService, ProfileService>();
            services.AddTransient<IClientStore, ClientStore>();
            services.AddTransient<ISeedAuthService, SeedAuthService>();

            services.AddTransient<IResourceOwnerPasswordValidator, ResourceOwnerPasswordValidator>()
                    .AddTransient<IProfileService, ProfileService>();

            services.AddIdentityServer(options =>
                {
                    options.UserInteraction.LoginUrl = "/Account/login";
                    options.UserInteraction.LogoutUrl = "/Account/logout";
                })
                // Replace with your certificate's thumbPrint, path, and password
                .AddSigningCredential(
                    CertificateLoader.Load(
                        "701480955FFC6E5423A267A37F5968E28E4FF31B",
                        Path.Combine(_env.ContentRootPath, "Certificates", "example.pfx"),
                        "OidcTemplate",
                        false))                
                //.AddDeveloperSigningCredential()
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                        builder.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));

                    // this enables automatic token cleanup. this is optional.
                    options.EnableTokenCleanup = true;
                    options.TokenCleanupInterval = 30; // interval in seconds
                }).
                AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                        builder.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                })                
                .AddAspNetIdentity<ApplicationUser>()
                .AddProfileService<ProfileService>();

            /////////////////// Configuration for Auth Server API ////////////////////////////////////////

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
                   options.ApiName = "internal_auth_api";
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

            services.AddTransient<IUnitOfWork, SiUnitOfWork>();
            
            /////////////////// Configuration for Auth Server API ////////////////////////////////////////


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseStaticFiles();
            app.UseIdentityServer();
            app.UseMvcWithDefaultRoute();
        }
    }
}
