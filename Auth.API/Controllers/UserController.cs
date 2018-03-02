using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Auth.Data.Access;
using Auth.Domain.Authentication;
using Auth.Domain.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace Auth.API.Controllers
{
    [Produces("application/json")]
    [Route("api/User")]
    public class UserController : Controller
    {
        private readonly IUnitOfWork _uow;
        private readonly EntityManager _em;
        //private readonly UserManager<ApplicationUser> _userManager;


        public UserController(EntityManager em, IUnitOfWork uow) { 
            //, UserManager<ApplicationUser> userManager) {
            _uow = uow;
            //_userManager = userManager;

            _em = em;
        }
        

        //[Authorize(Policy = DomainPolicies.SuperAdmin)]
        //[Authorize(Policy = DomainPolicies)]
        [Authorize]
        [HttpGet]
        [Route("Get")]
        public async Task<ApplicationUser> GetById(string userId)
        {
            var user = await _uow.ApplicationUser.FindOneAsync(u => u.Id == userId);
            user.OrganisationAccount = await _uow.OrganisationAccount.FindOneAsync(o => o.Id == user.OrganisationId);
            return user;
        }


        [Authorize]
        [HttpGet]
        [Route("GetAll")]
        public async Task<IEnumerable<ApplicationUser>> GetAll()
        {
            var userList = await _uow.ApplicationUser.FindAllAsync();
            return userList;
        }

        //public class ResetPasswordViewModel
        //{
        //    [Required]
        //    [EmailAddress]
        //    public string Email { get; set; }

        //    [Required]
        //    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        //    [DataType(DataType.Password)]
        //    public string Password { get; set; }

        //    [DataType(DataType.Password)]
        //    [Display(Name = "Confirm password")]
        //    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        //    public string ConfirmPassword { get; set; }

        //    public string Code { get; set; }
        //}


    }
}