using AutoMapper;
using IdentityServer.Traditional.Contract.Command;
using IdentityServer.Traditional.Messaging.Response;
using MediatR;
using Services.User;

namespace IdentityServer.Traditional.Contract.Handle
{
    public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, UpdateUserResponse>
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        public UpdateUserHandler(IUserService userService, IMapper mapper)
        {
            this._userService = userService;
            this._mapper = mapper;
        }
        public async Task<UpdateUserResponse> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            UpdateUserResponse response = new();

            try
            {
                var user = await _userService.FindByIdAsync(request.Request.ViewModel.Id);
                if (user is null)
                {
                    response.Failed();
                    response.FailedMessage("Bad Request");
                    return response;
                }


                user.UserName = request.Request.ViewModel.UserName;
                user.Email = request.Request.ViewModel.Email;

                if (!string.IsNullOrEmpty(request.Request.ViewModel.Password))
                {
                    user.PasswordHash = _userService.HashPassword(user, request.Request.ViewModel.Password);
                }

                var result = await _userService.UpdateUserAsync(user);

                if (request.Request.DoerId == request.Request.ViewModel.Id)
                {
                    //signout
                }

                if (!result.Succeeded)
                {
                    response.Failed();

                    response.FailedMessage(result.Errors.FirstOrDefault().Code);

                    return response;
                }

                response.Succeed();

                response.SuccessMessage();

                return response;
            }
            catch (Exception e)
            {
                response.Failed();

                response.FailedMessage();

                response.FailedMessage(e.Message);

                return response;
            }
        }
    }
}
