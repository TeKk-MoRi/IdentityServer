using AutoMapper;
using IdentityServer.Traditional.Models;
using IdentityServer.Traditional.ViewModels;

namespace IdentityServer.Traditional.Mapping
{
    public class UserMapprofile : Profile
    {
        public UserMapprofile()
        {
            CreateMap<ApplicationUser, UserViewModel>().ReverseMap();
            CreateMap<ApplicationUser, RegisterApplicationUserViewModel>().ReverseMap();
        }
    }
}
