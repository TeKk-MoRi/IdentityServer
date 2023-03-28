using IdentityServer.Traditional.Messaging.Requests;
using IdentityServer.Traditional.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using IdentityServer.Traditional.Extensions;
using IdentityServer.Traditional.Messaging.Base;
using IdentityServer.Traditional.Contract.Command;
using IdentityServer.Traditional.Messaging.Response;

namespace IdentityServer.Traditional.Controllers
{
    public class AuthenticationController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly IJWTExtension _jWTExtension;
        public AuthenticationController(IMediator mediator, IJWTExtension jWTExtension)
        {
            _mediator = mediator;
            this._jWTExtension = jWTExtension;
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var res = await _mediator.Send(new LoginCommand(new LoginRequest(model)));

            if (res.Result == null && !res.IsSucceed)
                return Response(res);

            if (res.Result != null)
            {
                var response = new GetTokenResponse();

                response.Result = await _jWTExtension.GenerateAuthenticationToken(res.Result, 7);

                return Response(response);
            }
            else
            {
                var response = new BaseResponse();

                response.Failed();

                response.FailedMessage("Login Failed!");

                return Response(response);
            }
        }


        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            var res = await _mediator.Send(new RegisterCommand(new RegisterRequest { ViewModel = model }));

            return Response(res);
        }
    }
}
