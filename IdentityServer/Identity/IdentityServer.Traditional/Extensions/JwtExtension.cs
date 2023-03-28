using AutoMapper;
using IdentityServer.Traditional.Helper.Common;
using IdentityServer.Traditional.Models;
using IdentityServer.Traditional.ViewModels;
using Microsoft.IdentityModel.Tokens;
using Services.User;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IdentityServer.Traditional.Extensions
{
    public class JwtExtension : IJWTExtension
    {
        private IMapper _mapper;
        private IUserService _userService;
        public JwtExtension(IMapper mapper , IUserService userService)
        {
            this._mapper = mapper;  
            this._userService = userService;
        }
        public async Task<AuthenticationViewModel> GenerateAuthenticationToken(UserViewModel model, ushort days)
        {
            var user = _mapper.Map<ApplicationUser>(model);

            var userRoles = await _userService.GetRolesAsync(user);

            var authClaims =
                new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

            foreach (string role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(CommonHelper.GetConfigurationSetting("JWT:Secret")));

            var token = new JwtSecurityToken(
                issuer: CommonHelper.GetConfigurationSetting("JWT:ValidIssuer"),
                audience: CommonHelper.GetConfigurationSetting("JWT:ValidAudience"),
                expires: DateTime.Now.AddDays(days),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return new AuthenticationViewModel
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpireDate = token.ValidTo
            };
        }

        public async Task<UserViewModel> IsTokenValid(string token)
        {
            if (token is null)
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidAudience = CommonHelper.GetConfigurationSetting("JWT:ValidAudience"),
                ValidIssuer = CommonHelper.GetConfigurationSetting("JWT:ValidIssuer"),
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(CommonHelper.GetConfigurationSetting("JWT:Secret")))
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;

            if (jwtToken is null)
                return null;

            var id = jwtToken.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;

            if (id is null)
                return null;

            var result = _mapper.Map<UserViewModel>(await _userService.GetById(id));

            if (result is null)
                return null;

            return result;
        }

    }
}
