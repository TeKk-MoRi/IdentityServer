using IdentityServer.Traditional.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.User
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public RoleManager<IdentityRole> RoleManager
        {
            get
            {
                return _roleManager;
            }
        }

        public async Task<bool> CheckPassword(ApplicationUser user, string password)
        {
            return (await _userManager.CheckPasswordAsync(user, password));
        }

        public async Task<ApplicationUser> FindByUserameAsync(string name)
        {
            return await _userManager.FindByNameAsync(name);
        }

        public async Task<ApplicationUser> GetByUsername(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }

        public async Task<List<string>> GetRolesAsync(ApplicationUser user)
        {
            return (await _userManager.GetRolesAsync(user)).ToList();
        }
        public async Task<string> AddUser(ApplicationUser model, string password)
        {
            var result = await _userManager.CreateAsync(model, password);

            if (!result.Succeeded)
                return "";

            return model.Id;
        }
        public async Task<bool> CheckRoleExistance(string role)
        {
            return await _roleManager.RoleExistsAsync(role);
        }
        public async Task AddRole(string role)
        {
            var res = new IdentityRole(role);
            await _roleManager.CreateAsync(res);
        }
        public async Task AddRoleToUser(ApplicationUser user, string role)
        {
            await _userManager.AddToRoleAsync(user, role);
        }
        public async Task<ApplicationUser> GetById(string id)
        {
            var model = await _userManager.FindByIdAsync(id);

            if (model == null)
                return null;

            return model;
        }

        public async Task<ApplicationUser> FindByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }
        public string HashPassword(ApplicationUser user, string password)
        {
            return _userManager.PasswordHasher.HashPassword(user, password);
        }
        public async Task<IdentityResult> UpdateUserAsync(ApplicationUser user)
        {
            return await _userManager.UpdateAsync(user);
        }

        //public async Task<IdentityResult> DeleteUserById(string id)
        //{
        //    var user = await _userManager.FindByIdAsync(id);
        //    user.IsDeleted = true;
        //    return await _userManager.UpdateAsync(user);
        //}
        public async Task<List<ApplicationUser>> GetAllUsers(int take)
        {
            IQueryable<ApplicationUser> users;

            users = _userManager.Users.AsQueryable();

            return await users.Take(take).ToListAsync();
        }
    }
}
