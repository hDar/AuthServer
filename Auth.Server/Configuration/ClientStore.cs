using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.Extensions.Options;
using Auth.Data.Context;
using Auth.Domain.Authentication;
using Auth.Domain.Configuration;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using IdentityServer4.EntityFramework.Mappers;

namespace Auth.Server.Configuration
{
    public class ClientStore : IClientStore
    {
        private readonly IdentityContext _identityContext;
        private readonly DomainSettings _domainSettings;
        private readonly ConfigurationDbContext _configurationDbContext;

        public ClientStore(IdentityContext identityContext, IOptions<DomainSettings> domainSettings, ConfigurationDbContext configurationDbContext)
        {
            _identityContext = identityContext;
            _domainSettings = domainSettings.Value;
            _configurationDbContext = configurationDbContext;
        }

        public async Task<Client> FindClientByIdAsync(string clientId)
        {
            var query = _configurationDbContext.Clients
                       .Include(x => x.AllowedCorsOrigins)
                       .Include(x => x.AllowedGrantTypes)
                       .Include(x => x.AllowedScopes)
                       .Include(x => x.Claims)
                       .Include(x => x.ClientSecrets)
                       .Include(x => x.IdentityProviderRestrictions)
                       .Include(x => x.PostLogoutRedirectUris)
                       .Include(x => x.Properties)                       
                       .Include(x => x.RedirectUris);
            
            var client = await query.SingleOrDefaultAsync(x => x.ClientId == clientId);
           
            return client.ToModel();
        }
    }
}