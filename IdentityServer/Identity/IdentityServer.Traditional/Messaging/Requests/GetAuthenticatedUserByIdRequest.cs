using IdentityServer.Traditional.Messaging.Base;

namespace IdentityServer.Traditional.Messaging.Requests
{
    public class GetAuthenticatedUserByIdRequest : BaseApiRequest
    {
        public string UserId { get; set; }
    }
}
