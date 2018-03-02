using Auth.Domain.Configuration;
using IdentityServer4;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Auth.Domain.Authentication
{
    public static class Resources
    {

        public static IEnumerable<Client> OidcMvcClients(DomainSettings _domainSettings)
        {
            return new List<Client>
            {
                new Client
                {
                    Enabled = true,
                    ClientId = _domainSettings.Client.Id,
                    ClientName = _domainSettings.Client.Name,
                    ClientUri = $"{_domainSettings.Client.Url}",
                    //RequirePkce = false,                    
                    RequireConsent = false,
                    AllowAccessTokensViaBrowser = true,
                    AllowOfflineAccess = true,
                    AlwaysSendClientClaims = true,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    RefreshTokenUsage = TokenUsage.ReUse,
                    RefreshTokenExpiration = TokenExpiration.Absolute,
                    AbsoluteRefreshTokenLifetime = 157700000, // 5 years. TODO: Consider better approach
                    AccessTokenLifetime = 3600,
                    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,
                    // Disable client secret validation
                    //RequireClientSecret = false,
                    ClientSecrets = new List<Secret>
                    {
                        new Secret(_domainSettings.Client.Secret.Sha256())
                    },
                    RedirectUris = new List<string>
                    {
                         $"{_domainSettings.Client.Url}/signin-oidc"
                    },
                    PostLogoutRedirectUris = new List<string>
                    {
                        $"{_domainSettings.Client.Url}/signout-callback-oidc"
                    },
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        _domainSettings.Api.Id,
                        DomainScopes.Roles,
                        DomainScopes.MvcClientUser,
                        "internal_auth_api"
                        //DomainScopes.ApiKeys
                    },
                    // Embedded in token
                    //Claims = new List<Claim>
                    //{
                    //    //new Claim(DomainClaimTypes.LiveEnabled, "true")
                    //}
                },
                new Client
                {
                    ClientId = "console.client",
                    ClientName = "Console Client",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = new List<Secret>
                    {
                        new Secret(_domainSettings.Client.Secret.Sha256())
                    },
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        _domainSettings.Api.Id,
                        DomainScopes.Roles,
                        DomainScopes.MvcClientUser,
                        "internal_auth_api"                        
                    },
                    Enabled = true,
                 },
                 new Client
                 {
                    ClientId = "console.ro.client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                   ClientSecrets = new List<Secret>
                    {
                        new Secret(_domainSettings.Client.Secret.Sha256())
                    },
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        _domainSettings.Api.Id,
                        DomainScopes.Roles,
                        DomainScopes.MvcClientUser,
                        "internal_auth_api"
                    }
                 }
            };
        }


        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResource
                {
                    Name = DomainScopes.MvcClientUser,
                    DisplayName = "MVC client user",
                    Description = "Basic resource, to be adjusted for your application",
                    //UserClaims = new List<string>
                    //{
                    //    DomainClaimTypes.TestUserId,
                    //    DomainClaimTypes.LiveUserId,
                    //    DomainClaimTypes.LiveEnabled,
                    //    DomainClaimTypes.SomeClaim,
                    //    DomainClaimTypes.AnotherClaim
                    //}
                },
                 new IdentityResource(DomainScopes.Roles, new List<string> {DomainClaimTypes.Role})
            };
        }

        public static IEnumerable<ApiResource> GetApis(WebResource api)
        {
            return new List<ApiResource>
            {
                new ApiResource
                {
                    Name = api.Id,
                    DisplayName = api.Name,
                    Description = "Protected Auth Api",
                    ApiSecrets = new List<Secret> {new Secret(api.Secret.Sha256())},
                    UserClaims = new List<string>
                    {
                        DomainClaimTypes.Role,
                        //DomainClaimTypes.TestUserId,
                        //DomainClaimTypes.LiveUserId,
                        //DomainClaimTypes.LiveEnabled,
                        //DomainClaimTypes.SomeClaim
                    },
                    Scopes = new List<Scope>
                    {
                        new Scope(){
                            Name = api.Id, // Should match name of ApiResource.Name
                            DisplayName = api.Name,
                            UserClaims = new List<string>{
                                DomainClaimTypes.Role
                            }
                        }, 

                        //new Scope
                        //{
                        //    Name = DomainScopes.ApiKeys,
                        //    DisplayName = "API Keys",
                        //    Description = "Access to the API keys."
                        //}
                    }
                },
                new ApiResource
                {
                    Name = "internal_auth_api",
                    DisplayName = "Auth  server API",
                    Description = "Protected Auth Api",
                    ApiSecrets = new List<Secret> {new Secret(api.Secret.Sha256())},
                    UserClaims = new List<string>
                    {
                        DomainClaimTypes.Role                       
                    },
                    Scopes = new List<Scope>
                    {
                        new Scope(){
                            Name = "internal_auth_api", // Should match name of ApiResource.Name
                            DisplayName = "Auth  server API",
                            UserClaims = new List<string>{
                                DomainClaimTypes.Role
                            }
                        }
                    }
                }
            };
        }
    }
}
