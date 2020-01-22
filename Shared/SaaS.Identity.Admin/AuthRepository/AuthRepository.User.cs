using Microsoft.AspNet.Identity;
using SaaS.Data.Entities.Admin.View.Oauth;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SaaS.Identity.Admin
{
    public partial class AuthRepository
    {
        public async Task<User> UserGetAsync(Guid userId)
        {
            return await _context.UserGetAsync(userId);
        }
        public async Task<User> UserGetAsync(string login)
        {
            return await _context.UserGetAsync(login);
        }
        public async Task UserSetActiveAsync(Guid userId, bool isActive)
        {
            await _context.UserSetActiveAsync(userId, isActive);
        }

        public async Task<IdentityResult> UserChangePasswordAsync(Guid userId, string oldPassword, string newPassword)
        {
            return await _userManager.ChangePasswordAsync(userId, oldPassword, newPassword);
        }

        public async Task<ViewUser> ViewUserGetAsync(Guid userId)
        {
            return await _context.ViewUserGetAsync(userId);
        }
        public async Task<ViewUser> ViewUserGetAsync(string login, string password)
        {
            return await _context.ViewUserGetAsync(login, password);
        }
        public async Task<List<ViewUser>> ViewUsersGetAsync()
        {
            return await _context.ViewUsersGetAsync();
        }
        public async Task ViewUserSetAsync(ViewUser user)
        {
            await _context.ViewUserSetAsync(user);
        }
        public async Task ViewUserInsertAsync(ViewUser user)
        {
            await _context.ViewUserInsertAsync(user);
        }
         public async Task<string> AccountGDPRDeleteAsync(string email)
        {
            var result=await _context.AccountGDPRDeleteAsync(email);
            return result;
        }
    }
}