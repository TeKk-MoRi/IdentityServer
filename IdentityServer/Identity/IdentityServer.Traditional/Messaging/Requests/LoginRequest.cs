using IdentityServer.Traditional.Messaging.Base;
using IdentityServer.Traditional.ViewModels;

namespace IdentityServer.Traditional.Messaging.Requests
{
    public class LoginRequest : BaseApiRequest<LoginViewModel>
    {
        public LoginRequest(LoginViewModel viewModel)
        {
            ViewModel = viewModel;
        }
    }
}
