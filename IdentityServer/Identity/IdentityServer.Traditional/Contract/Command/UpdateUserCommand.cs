using IdentityServer.Traditional.Messaging.Requests;
using IdentityServer.Traditional.Messaging.Response;
using MediatR;

namespace IdentityServer.Traditional.Contract.Command
{
    public record UpdateUserCommand(UpdateUserRequest Request) : IRequest<UpdateUserResponse>;
}
