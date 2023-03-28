using AutoMapper;
using IdentityServer.Traditional.Contract.Query;
using IdentityServer.Traditional.Messaging.Response;
using IdentityServer.Traditional.ViewModels;
using MediatR;
using Services.User;

namespace IdentityServer.Traditional.Contract.Handle
{
    public class GetAllUsersHandler : IRequestHandler<GetAllUsersQuery, GetAllUsersResponse>
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        public GetAllUsersHandler(IUserService userService, IMapper mapper)
        {
            this._userService = userService;
            this._mapper = mapper;
        }
        public async Task<GetAllUsersResponse> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            GetAllUsersResponse response = new();

            try
            {
                var result = await _userService.GetAllUsers(request.Request.ViewModel.Take);
                response.Result = _mapper.Map<List<UserViewModel>>(result);
                response.Succeed();
            }
            catch (Exception e)
            {
                response.Failed();
                response.FailedMessage();
                response.FailedMessage(e.Message);
            }

            return response;
        }
    }
}
