using Auth.server.Models.AccountViewModels;

namespace Auth.Server.Models.AccountViewModels
{
    public class LogoutViewModel : LogoutInputModel
    {
        public bool ShowLogoutPrompt { get; set; }
    }
}
