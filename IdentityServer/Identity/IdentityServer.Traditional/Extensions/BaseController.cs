using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using IdentityServer.Traditional.Helper;
using IdentityServer.Traditional.Helper.Common;
using IdentityServer.Traditional.Messaging.Base;

namespace IdentityServer.Traditional.Extensions
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BaseController : ControllerBase
    {

        public ApiHelper APIHelper
        {
            get
            {
                return new ApiHelper(HttpContext, (IMediator)HttpContext.RequestServices.GetService(typeof(IMediator)));
            }
        }
        public new IActionResult Response<T>(T model, HttpStatusCode statusCode = default(HttpStatusCode)) where T : BaseApiResponse
        {
            var sCode = statusCode == default(HttpStatusCode) ? model.IsSucceed ? HttpStatusCode.OK : model.IsAccessDenied ? HttpStatusCode.Forbidden : HttpStatusCode.BadRequest : statusCode;

            return new ObjectResult(model) { StatusCode = (int)sCode };
        }
    }
}
