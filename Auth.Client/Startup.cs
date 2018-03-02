﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Auth.Domain.Authentication;
using Auth.Domain.Configuration;
using IdentityServer4;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Client
{
    public class Startup
    {
        private readonly int _sslPort = 443;

        public Startup(IHostingEnvironment env, IConfiguration configuration)
        {
            Configuration = configuration;

            if (env.IsDevelopment()) {
                var launchConfiguration = new ConfigurationBuilder()
                    .SetBasePath(env.ContentRootPath)
                    .AddJsonFile(Path.Combine("Properties", "launchSettings.json"))
                    .Build();

                _sslPort = launchConfiguration.GetValue<int>("iisSettings::iisExpress::sslPort");
            }
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var domainSettings = new DomainSettings();
            var section = Configuration.GetSection(nameof(DomainSettings));
            section.Bind(domainSettings);
            services.Configure<DomainSettings>(options => section.Bind(options));
            
            services.AddMvc(options => options.SslPort = _sslPort);

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication(
                    options =>
                    {
                        options.DefaultScheme = "Cookies";
                        options.DefaultChallengeScheme = "oidc";
                    })
                .AddCookie("Cookies")
                .AddOpenIdConnect("oidc", options =>
                {
                    options.SignInScheme = "Cookies";
                    options.Authority = domainSettings.Auth.Url;
                    options.RequireHttpsMetadata = false;
                    options.ClientId = domainSettings.Client.Id;
                    options.ClientSecret = domainSettings.Client.Secret;
                    options.SaveTokens = true;

                    options.ResponseType = "code id_token";
                    options.Scope.Add(IdentityServerConstants.StandardScopes.Email);
                    options.Scope.Add(IdentityServerConstants.StandardScopes.OpenId);
                    options.Scope.Add(IdentityServerConstants.StandardScopes.OfflineAccess);
                    options.SaveTokens = true;
                    options.GetClaimsFromUserInfoEndpoint = true;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = "name",
                        RoleClaimType = "role"
                    };
                    options.Scope.Add(domainSettings.Api.Id);
                    options.Scope.Add("internal_auth_api");
                    options.Scope.Add(DomainScopes.Roles);
                    options.Scope.Add(DomainScopes.MvcClientUser);
                    //options.Scope.Add(DomainScopes.ApiKeys);

                    options.Events = new OpenIdConnectEvents
                    {
                        OnRemoteFailure = context =>
                        {
                            Console.WriteLine(context.Failure.Message +
                                              " Failiure during authentication - Remote failure.");
                            return Task.FromResult(0);
                        }
                    };
                });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                //app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                //{
                //    HotModuleReplacement = true
                //});
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseAuthentication();

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }
    }
}
