using IdentityServer.Traditional.Messaging.Base;
using IdentityServer.Traditional.ViewModels;

namespace IdentityServer.Traditional.Messaging.Requests
{
    public class UpdateUserRequest : BaseApiRequest<UpdateUserViewModel>
    {
        public string DoerId { get; set; }
    }
}
