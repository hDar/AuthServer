using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using Auth.Data.Context;
using Auth.Domain.Authentication;
using Auth.Domain.Entities.Identity;
using IdentityServer4.Services;
using System;

namespace Auth.Server.Configuration
{
    /// <inheritdoc />
    /// <summary>
    /// AspNet profile service
    /// </summary>
    //public class ProfileService : IdentityServer4.AspNetIdentity.ProfileService<ApplicationUser>
    //{
    //    private readonly IdentityContext _context;
    //    private readonly UserManager<ApplicationUser> _userManager;

    //    public ProfileService(UserManager<ApplicationUser> userManager,
    //        IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory, IdentityContext context) : base(userManager,
    //        claimsFactory)
    //    {
    //        _context = context;
    //        _userManager = userManager;
    //    }

    //    public override async Task GetProfileDataAsync(ProfileDataRequestContext context)
    //    {
    //        await base.GetProfileDataAsync(context);

    //        var userData = _context.Users.FirstOrDefault(x => x.Id == context.Subject.FindFirstValue(JwtClaimTypes.Subject));


    //        var userName = userData.FirstName.IsNullOrEmpty() || userData.LastName.IsNullOrEmpty()
    //            ? userData.Email
    //            : $"{userData.FirstName} {userData.LastName}";

    //        var userRole = _userManager.GetRolesAsync(userData).Result.First();

    //        var userClaims = new List<Claim>
    //        {
    //            //new Claim(DomainClaimTypes.TestUserId, "some test_user_id"),
    //            //new Claim(DomainClaimTypes.LiveUserId, "some live_user_id"),
    //            //new Claim(DomainClaimTypes.LiveEnabled, "true"),
    //            //new Claim(DomainClaimTypes.SomeClaim, "some_claim value"),
    //            //new Claim(DomainClaimTypes.AnotherClaim, "another_claim value"),
    //            new Claim("name", userName),
    //            new Claim("organisationId", userData.OrganisationId),
    //            //new Claim(DomainClaimTypes.Role, userRole)
    //        };

    //        context.AddRequestedClaims(userClaims);
    //    }
    //}


    public class ProfileService : IProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager;


        public ProfileService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            try
            {
                var subjectId = context.Subject.GetSubjectId();
                var user = _userManager.FindByIdAsync(subjectId).Result;
                var userRole = _userManager.GetRolesAsync(user).Result.First();

                var claims = new List<Claim>
                {
                    new Claim(JwtClaimTypes.Subject, user.Id.ToString()),
				    //add as many claims as you want!new Claim(JwtClaimTypes.Email, user.Email),new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean)
                    new Claim(JwtClaimTypes.Role, userRole),
                    new Claim(JwtClaimTypes.Email, user.Email),
                    new Claim("organisationId", user.OrganisationId)
                };

                context.IssuedClaims = claims;
                return Task.FromResult(0);
            }
            catch (Exception x)
            {
                return Task.FromResult(0);
            }
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            var user = _userManager.FindByIdAsync(context.Subject.GetSubjectId()).Result; //_repository.GetUserById(context.Subject.GetSubjectId());
            context.IsActive = (user != null);//&& user.Active;
            return Task.FromResult(0);
        }
    }
}