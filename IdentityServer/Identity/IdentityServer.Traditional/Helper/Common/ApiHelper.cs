using IdentityServer.Traditional.Contract.Query;
using IdentityServer.Traditional.ViewModels;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Runtime;
using System;
using IdentityServer.Traditional.Helper.Redis;
using IdentityServer.Traditional.Messaging.Requests;
using UAParser;

namespace IdentityServer.Traditional.Helper.Common
{
    public class ApiHelper
    {
        private HttpContext _context;
        private readonly IMediator _mediator;

        public ApiHelper(HttpContext context, IMediator mediator)
        {
            this._context = context;
            this._mediator = mediator;
        }

        public async Task RefreshUser()
        {
            var resUser = await _mediator.Send(new GetAuthenticatedUserByIdQuery(new GetAuthenticatedUserByIdRequest { UserId = GetRequestedUserInformation().Id }));

            await SetRequestedUserInformation(resUser.Result);
        }
        public UserViewModel GetRequestedUserInformation()
        {
            return _context?.Items["User"] as UserViewModel;
        }

        public async Task SetRequestedUserInformation(UserViewModel model)
        {
            var cache = (IDistributedCache)_context.RequestServices.GetService(typeof(IDistributedCache));

            string recordKey = "UserIdentity" + "_" + GetToken();

            await cache.SetRecordAsync(recordKey, model, TimeSpan.FromDays(7));

            _context.Items["User"] = model;
        }

        public bool IsRequestHttps()
        {
            return _context.Request.IsHttps;
        }

        public string GetUserRequestIP()
        {
            var ip = _context.Request.HttpContext.Connection.RemoteIpAddress.ToString();

            if (string.IsNullOrEmpty(ip) || ip == "::1")
                return "127.0.0.1";

            return ip;
        }

        public string GetUserRequestPort()
        {
            return _context.Request.HttpContext.Connection.RemotePort.ToString();
        }

        public async Task<IPInfo> GetUserRequestIPInformation()
        {
            return await GetIPInformation(GetUserRequestIP());
        }

        public async Task<IPInfo> GetIPInformation(string ip)
        {
            HttpClient client = new HttpClient();

            var response = await client.GetStringAsync(@$"https://ipapi.co/{ip}/json/");

            return JsonConvert.DeserializeObject<IPInfo>(response);
        }

        public async Task<string> GetUserRequestISP()
        {
            return (await GetUserRequestIPInformation()).Org;
        }

        public async Task<string> GetUserRequestCountryName()
        {
            return (await GetUserRequestIPInformation()).Country_name;
        }

        public async Task<string> GetUserRequestCountryISO()
        {
            return (await GetUserRequestIPInformation()).Country;
        }

        public async Task<string> GetUserRequestIPVersion()
        {
            return (await GetUserRequestIPInformation()).Version;
        }

        public async Task<string> GetUserRequestTimezone()
        {
            return (await GetUserRequestIPInformation()).Timezone;
        }

        public async Task<string> GetUserRequestLanguage()
        {
            return (await GetUserRequestIPInformation()).Languages.Split(",")[0];
        }

        public async Task<string> GetUserRequestCountryTLD()
        {
            return (await GetUserRequestIPInformation()).Country_tld;
        }

        public UAParser.ClientInfo GetUserRequestClientInfo()
        {
            var userAgent = _context.Request.Headers["User-Agent"];

            var uaParser = Parser.GetDefault();

            return uaParser.Parse(userAgent);
        }

        public Device GetUserRequestDevice()
        {
            return GetUserRequestClientInfo().Device;
        }

        public OS GetUserRequestOS()
        {
            return GetUserRequestClientInfo().OS;
        }

        private string GetToken()
        {
            return _context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        }
    }
}
