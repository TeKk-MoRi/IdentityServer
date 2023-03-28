using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.Traditional.Messaging.Base;
using IdentityServer.Traditional.Extensions;
using IdentityServer.Traditional.ViewModels;
using IdentityServer.Traditional.Helper.Redis;

namespace IdentityServer.Traditional.Middleware
{
    public class JWTMiddleware
    {
        private readonly RequestDelegate _next;
        private bool IsValid = true;
        private readonly IHttpContextAccessor _httpContext;

        public JWTMiddleware(RequestDelegate next, IHttpContextAccessor httpContext)
        {
            _next = next;
            _httpContext = httpContext;

        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                var _hostingEnvironmen = (IWebHostEnvironment)_httpContext.HttpContext.RequestServices.GetService(typeof(IWebHostEnvironment));

                var whitePathes = File.ReadAllLines(_hostingEnvironmen.ContentRootPath + @"\WhitePath\WhitePathes.txt").Select(s => s.ToLowerInvariant());

                var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

                var path = context.Request.Path.Value.ToLower().Trim();
                if (path.EndsWith('/'))
                    path = path.Remove(path.Length - 1);

                if (whitePathes.Contains(path) || path.StartsWith("/swagger/") || path == "")
                    await _next(context);
                else
                {
                    if (token is null)
                    {
                        var response = new BaseResponse();
                        response.Failed();
                        response.FailedMessage("Forbidden !");

                        context.Response.StatusCode = 403;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(response));

                        return;
                    }

                    await attachUserToContextAsync(context, token, path);

                    if (!IsValid)
                    {
                        var response = new BaseResponse();
                        response.Failed();
                        response.FailedMessage("Bad Request");

                        context.Response.StatusCode = 400;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(response));

                        return;
                    }

                    await _next(context);
                }
            }
            catch (Exception x)
            {

                throw;
            }

        }

        private async Task attachUserToContextAsync(HttpContext context, string token, string path)
        {
            IsValid = true;
            var cache = (IDistributedCache)_httpContext.HttpContext.RequestServices.GetService(typeof(IDistributedCache));
            var _jWTExtension = (IJWTExtension)_httpContext.HttpContext.RequestServices.GetService(typeof(IJWTExtension));

            string recordKey = "UserIdentity" + "_" + token;
            var user = await cache.GetRecordAsync<UserViewModel>(recordKey);
            if (user is null)
            {
                user = await _jWTExtension.IsTokenValid(token);
                await cache.SetRecordAsync(recordKey, user, TimeSpan.FromHours(2));
            }


            if ((!string.IsNullOrWhiteSpace(token) || !string.IsNullOrEmpty(token)) && user == null)
            {
                IsValid = false;
            }

            context.Items["User"] = user;
        }
    }
}
