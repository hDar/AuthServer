using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Auth.server.Models.BindingModels
{
    public class AccountBindingModel
    {
        [Required]
        public string Name { get; set; }
        public string Region { get; set; }
        public string Data { get; set; }

        public string AddedBy { get; set; }
              

        public UserBindingModel User { get; set; }
    }

    public class UserBindingModel {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Civility { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public bool IsActive { get; set; }

        public string OrganisationId { get; set; }

        public string ApprovedBy { get; set; }

        public bool IsPasswordTemporary { get; set; }

        public string UserRole { get; set; }
    }

    public class UpdateUserBindingModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public bool IsPasswordTemporary { get; set; }
    }

}
