

using IdentityServer.Traditional.ViewModels;

namespace IdentityServer.Traditional.Extensions
{
    public interface IJWTExtension
    {
        Task<AuthenticationViewModel> GenerateAuthenticationToken(UserViewModel model, ushort days);
        Task<UserViewModel> IsTokenValid(string token);
    }
}
