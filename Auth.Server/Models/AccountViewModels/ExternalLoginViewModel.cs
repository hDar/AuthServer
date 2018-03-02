using System.ComponentModel.DataAnnotations;


namespace Auth.Server.Models.AccountViewModels
{
    public class ExternalLoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
