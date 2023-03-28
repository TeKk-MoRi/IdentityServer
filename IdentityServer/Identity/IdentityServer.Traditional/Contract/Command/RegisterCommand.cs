using IdentityServer.Traditional.Messaging.Requests;
using IdentityServer.Traditional.Messaging.Response;
using MediatR;

namespace IdentityServer.Traditional.Contract.Command
{
    public record RegisterCommand(RegisterRequest Request) : IRequest<RegisterResponse>;
}
