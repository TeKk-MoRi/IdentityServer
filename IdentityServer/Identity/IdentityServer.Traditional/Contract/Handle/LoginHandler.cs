using AutoMapper;
using IdentityServer.Traditional.Contract.Command;
using IdentityServer.Traditional.Messaging.Response;
using IdentityServer.Traditional.Models;
using IdentityServer.Traditional.ViewModels;
using MediatR;
using Services.User;

namespace IdentityServer.Traditional.Contract.Handle
{
    public class LoginHandler : IRequestHandler<LoginCommand, LoginResponse>
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        public LoginHandler(IUserService userService, IMapper mapper)
        {
            this._userService = userService;
            this._mapper = mapper;
        }
        public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            LoginResponse response = new();
            try
            {
                ApplicationUser user = await GetUserExistenceStatus(request.Request.ViewModel.Username);

                if (user != null)
                {
                    //Go To Login Step 2
                    var isPassEqual = await LoginStepTwo(user, request.Request.ViewModel.Password);

                    if (isPassEqual)
                    {
                        response.Succeed();
                        response.SuccessMessage("User Can be logged in");
                        response.Result = _mapper.Map<UserViewModel>(user);
                    }
                    else
                    {
                        response.Failed();
                        response.FailedMessage("Password is wrong");
                        response.Result = null;
                    }
                }
                return response;
            }
            catch (Exception ex)
            {
                response.Failed();
                response.FailedMessage();
                response.FailedMessage(ex.Message);
                return response;
            }
        }

        private async Task<ApplicationUser> GetUserExistenceStatus(string userName)
        {
            return await _userService.GetByUsername(userName);
        }

        private async Task<bool> LoginStepTwo(ApplicationUser user, string password)
        {
            if (user != null)
            {
                if (password != null)
                {
                    bool isPassEqual = await _userService.CheckPassword(user, password);
                    if (isPassEqual)
                    {
                        return true;
                    }
                    return false;
                }
                return false;
            }
            return false;
        }
    }
}
