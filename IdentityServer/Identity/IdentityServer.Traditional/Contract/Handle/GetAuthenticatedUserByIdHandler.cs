using AutoMapper;
using IdentityServer.Traditional.Contract.Query;
using IdentityServer.Traditional.Messaging.Response;
using IdentityServer.Traditional.ViewModels;
using MediatR;
using Services.User;

namespace IdentityServer.Traditional.Contract.Handle
{
    public class GetAuthenticatedUserByIdHandler : IRequestHandler<GetAuthenticatedUserByIdQuery, GetAuthenticatedUserByIdResponse>
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        public GetAuthenticatedUserByIdHandler(IUserService userService, IMapper mapper)
        {
            this._userService = userService;
            this._mapper = mapper;
        }

        public async Task<GetAuthenticatedUserByIdResponse> Handle(GetAuthenticatedUserByIdQuery request, CancellationToken cancellationToken)
        {
            GetAuthenticatedUserByIdResponse response = new();

            try
            {
                var userViewModel = new UserViewModel();
                var user = await _userService.GetById(request.Request.UserId);
                if (user is null)
                {
                    response.Failed();
                    response.FailedMessage("user not found");
                }
                else
                {
                    userViewModel = _mapper.Map<UserViewModel>(user);
                    response.Succeed();
                    response.Result = userViewModel;
                }
                return response;
            }

            catch (System.Exception ex)
            {
                response.Failed();
                response.FailedMessage();
                response.FailedMessage(ex.Message);
                return response;
            }
        }
    }
}

