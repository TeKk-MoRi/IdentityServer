using IdentityServer.Traditional.Messaging.Requests;
using IdentityServer.Traditional.Messaging.Response;
using MediatR;

namespace IdentityServer.Traditional.Contract.Query
{
    public record GetAuthenticatedUserByIdQuery(GetAuthenticatedUserByIdRequest Request) : IRequest<GetAuthenticatedUserByIdResponse>;
}
