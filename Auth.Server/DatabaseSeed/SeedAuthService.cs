using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Auth.Domain.Authentication;
using Auth.Domain.Entities.Identity;
using IdentityServer4.EntityFramework.DbContexts;
using Auth.Domain.Configuration;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Stores;
using Auth.Data.Context;

namespace Auth.Server.DatabaseSeed
{
    public class SeedAuthService : ISeedAuthService
    {
        private readonly ILogger<SeedAuthService> _logger;        
        public SeedAuthService(ILogger<SeedAuthService> logger)
        {
            _logger = logger;            
        }

        public async Task SeedAuthDatabase(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            try
            {
                if (roleManager.Roles.Any() == false)
                {
                    await roleManager.CreateAsync(new IdentityRole(DomainRoles.SuperAdmin));
                    await roleManager.CreateAsync(new IdentityRole(DomainRoles.Admin));
                    await roleManager.CreateAsync(new IdentityRole(DomainRoles.RootUser));
                    await roleManager.CreateAsync(new IdentityRole(DomainRoles.Doctor));
                    await roleManager.CreateAsync(new IdentityRole(DomainRoles.Centre));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to seed authentication database.");
            }

            var dbContext = serviceProvider.GetRequiredService<IdentityContext>();
            var organisationAccount = new OrganisationAccount()
            {
                Name = "Straxcorp PTY Ltd",
                Region = "AU",
                Data = "",
                AddedDate = DateTime.UtcNow,
                AddedBy = "0"
            };

            if (!dbContext.OrganisationAccount.Any(u => u.Name == organisationAccount.Name))
            {
                try
                {
                    var result = await dbContext.AddAsync<OrganisationAccount>(organisationAccount);
                    await dbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to seed authentication database.");
                }
            }

            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            try
            {
                var orgnisationAccount = dbContext.OrganisationAccount.FirstOrDefault();

                var adminUser = new ApplicationUser
                {
                    Civility = "Mr.",
                    FirstName = "Admin",
                    LastName = "Straxcorp",
                    OrganisationId = "",
                    IsActive = true,
                    LastLoggedOn = System.DateTime.Now,
                    ApprovedBy = null,
                    DateAdded = DateTime.Now,
                    DateModified = DateTime.Now,
                    Email = "straximages.dev.team.au@gmail.com",
                    NormalizedEmail = "straximages.dev.team.au@gmail.com",
                    UserName = "straximages.dev.team.au@gmail.com",
                    NormalizedUserName = "straximages.dev.team.au@gmail.com",
                    PhoneNumber = "+61123659863",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString("D"),
                    IsPasswordTemporary = false,
                    OrganisationAccount = orgnisationAccount
                };

                if (await userManager.FindByEmailAsync(adminUser.Email) == null)
                {
                    var password = new PasswordHasher<ApplicationUser>();
                    var hashed = password.HashPassword(adminUser, "Straximages#2017");
                    adminUser.PasswordHash = hashed;
                    var user = await userManager.CreateAsync(adminUser);
                    if (user.Succeeded)
                    {
                        var addToRole = await userManager.AddToRoleAsync(adminUser, DomainRoles.SuperAdmin);
                    }
                }
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Failed to seed authentication database.");
            }


            var configurationContext = serviceProvider.GetRequiredService<ConfigurationDbContext>();
            try
            {
                //Seeding IDENTITY SERVER Configuration data               
                var _domainSettings = new DomainSettings()
                {
                    Client = new WebResource()
                    {
                        Id = "oidc_web",
                        Name = "Mvc Client",
                        Secret = "secret",
                        Url = "http://localhost:5002"
                    },
                    Api = new WebResource()
                    {
                        Id = "auth_api",
                        Name = "Auth API",
                        Secret = "secret",
                        Url = "http://localhost:5001"
                    },
                    Auth = new WebResource
                    {
                        Id = "",
                        Secret = "",
                        Url = "http://localhost:5000"
                    }
                };

                if (!configurationContext.IdentityResources.Any())
                {
                    foreach (var resource in Resources.GetIdentityResources())
                    {
                        configurationContext.IdentityResources.Add(resource.ToEntity());
                    }
                    configurationContext.SaveChanges();
                }

                if (!configurationContext.ApiResources.Any())
                {
                    foreach (var resource in Resources.GetApis(_domainSettings.Api))
                    {
                        configurationContext.ApiResources.Add(resource.ToEntity());
                    }
                    configurationContext.SaveChanges();
                }

                foreach (var client in Resources.OidcMvcClients(_domainSettings))
                {
                    var result = configurationContext.Clients.Where(c => c.ClientId == client.ClientId).FirstOrDefault();
                    if (result == null)
                    {
                        configurationContext.Clients.Add(client.ToEntity());
                        configurationContext.SaveChanges();
                    }
                    
                }
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Failed to seed authentication database.");
            }
            
        }
    }
}