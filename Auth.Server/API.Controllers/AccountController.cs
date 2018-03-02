using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auth.Data.Access;
using Auth.Domain.Authentication;
using Auth.Domain.Entities.Identity;
using Auth.server.Models.BindingModels;
using Auth.Server.Models.AccountViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Auth.Server.API.Controllers
{
    [Produces("application/json")]
    [Route("api/Account")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _uow;

        public AccountController(UserManager<ApplicationUser> userManager, IUnitOfWork uow) {
            _userManager = userManager;
            _uow = uow;
        }

        [Route("AddRootUser")]
        [HttpPost]
        //[Authorize(Policy = DomainPolicies.SuperAdmin)]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> AddRootUser([FromBody]AccountBindingModel model)
        {
            if (ModelState.IsValid)
            {
                var checkOrg = await _uow.OrganisationAccount.FindOneAsync(o => o.Name == model.Name);
                if (checkOrg != null)
                {
                    return BadRequest("Organisation Account already exists");
                }

                var orgAccount = new OrganisationAccount()
                {
                    Name = model.Name,
                    Region = model.Region,
                    Data = model.Data,
                    AddedBy = model.AddedBy,
                    AddedDate = DateTime.UtcNow
                };

                var resultOrg = _uow.OrganisationAccount.Add(orgAccount);

                var user = new ApplicationUser
                {
                    UserName = model.User.Email,
                    Email = model.User.Email,
                    Civility = model.User.Civility,
                    FirstName = model.User.FirstName,
                    LastName = model.User.LastName,
                    IsActive = model.User.IsActive,
                    OrganisationId = resultOrg.Id,
                    IsPasswordTemporary = model.User.IsPasswordTemporary,
                    DateAdded = DateTime.UtcNow,
                    DateModified = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, model.User.Password);

                var addedUser = await _userManager.FindByEmailAsync(user.Email);

                var roleTester = await _userManager.IsInRoleAsync(addedUser, model.User.UserRole);

                if (!await _userManager.IsInRoleAsync(addedUser, model.User.UserRole))
                {
                    result = await _userManager.AddToRoleAsync(addedUser, model.User.UserRole);
                }

                if (result.Succeeded)
                {
                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                    //await _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);                    
                }

                return Ok(addedUser);
            }
            return BadRequest();
        }


        [Route("AddUser")]
        [HttpPost]
        //[Authorize(Policy = DomainPolicies.SuperAdmin)]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> AddUser([FromBody]UserBindingModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Civility = model.Civility,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    IsActive = model.IsActive,
                    OrganisationId = model.OrganisationId,
                    IsPasswordTemporary = model.IsPasswordTemporary,
                    DateAdded = DateTime.UtcNow,
                    DateModified = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                var addedUser = await _userManager.FindByEmailAsync(user.Email);

                var roleTester = await _userManager.IsInRoleAsync(addedUser, model.UserRole);

                if (!await _userManager.IsInRoleAsync(addedUser, model.UserRole))
                {
                    result = await _userManager.AddToRoleAsync(addedUser, model.UserRole);
                }

                if (result.Succeeded)
                {
                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                    //await _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);                    
                }

                return Ok(addedUser);
            }
            return BadRequest();
        }



        [Route("Update")]
        [HttpPost]        
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> UpdateUserProfile([FromBody]UpdateUserBindingModel model)
        {
            if (!ModelState.IsValid) {
                return BadRequest("Model validation failed");
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                //return RedirectToAction(nameof(ResetPasswordConfirmation));
                return BadRequest("User Not Found");
            }


            //user.UserName = model.Email;
            //user.Email = model.Email;
            //user.Civility = model.Civility;
            //user.FirstName = model.FirstName;
            //user.LastName = model.LastName;
            user.IsActive = model.IsActive;
            user.IsPasswordTemporary = model.IsPasswordTemporary;                
            user.DateModified = DateTime.UtcNow;
            

            var result = await _userManager.UpdateAsync(user);
            var addedUser = await _userManager.FindByEmailAsync(user.Email);

            if (result.Succeeded)
            {
                return Ok(user);
                //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                //var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                //await _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);                    
            }
            return BadRequest("Request Failed");
        }


        [Route("ResetPassword")]
        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> ResetPassword([FromBody]ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                //return View(model);
                return BadRequest("Model validation failed");
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                //return RedirectToAction(nameof(ResetPasswordConfirmation));
                return BadRequest("User Not Found");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(user, token, model.Password);
            if (result.Succeeded)
            {                
                return Ok(user);
            }            
            return BadRequest("Request failed");
        }

        [Route("GetAll")]
        [HttpGet]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> GetAll(string userId)
        {           
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                //return RedirectToAction(nameof(ResetPasswordConfirmation));
                return BadRequest("User Not Found");
            }

            var userList = _userManager.Users.Where(u => u.Id != userId).ToList();

            if (userList.Count() > 0) {
                return Ok(userList);
            }

            return BadRequest("No users found");
        }


        [Route("GetOrgUsers")]
        [HttpGet]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult GetOrgUsers(string organisationId)
        {
            var userList =  _userManager.Users.Where(u => u.OrganisationId == organisationId);
            
            if (userList.Count() > 0)
            {
                return Ok(userList);
            }

            return BadRequest("No users found");
        }

        [Route("OrganisationUsers")]
        [HttpGet]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IEnumerable<ApplicationUser> GetUsers(string OrgnisationId)
        {
            var users = _userManager.Users.Where(u => u.OrganisationId == OrgnisationId).ToList();
            return users;
        }

    }
}