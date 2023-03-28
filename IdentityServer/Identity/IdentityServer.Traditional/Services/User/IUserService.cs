using IdentityServer.Traditional.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.User
{
    public interface IUserService
    {
        Task<List<string>> GetRolesAsync(ApplicationUser user);
        Task<ApplicationUser> GetByUsername(string userName);
        Task<bool> CheckPassword(ApplicationUser user, string password);
        Task<ApplicationUser> FindByUserameAsync(string name);
        Task<string> AddUser(ApplicationUser model, string password);
        Task<bool> CheckRoleExistance(string role);
        Task AddRole(string role);
        Task AddRoleToUser(ApplicationUser user, string role);
        Task<ApplicationUser> GetById(string id);
        Task<ApplicationUser> FindByIdAsync(string id);
        string HashPassword(ApplicationUser user, string password);
        Task<IdentityResult> UpdateUserAsync(ApplicationUser user);
        //Task<IdentityResult> DeleteUserById(string id);
        Task<List<ApplicationUser>> GetAllUsers(int take);
    }
}
