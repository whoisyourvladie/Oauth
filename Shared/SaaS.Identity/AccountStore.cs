using Microsoft.AspNet.Identity;
using SaaS.Data.Entities.Accounts;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SaaS.Identity
{
    internal class AccountStore<TUser> : IUserStore<TUser, Guid>, 
        IUserEmailStore<TUser, Guid>, 
        IUserPasswordStore<TUser, Guid>, 
        IUserClaimStore<TUser, Guid>
        where TUser : Account
    {
        private readonly AuthDbContext _context;

        internal AccountStore(AuthDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(TUser user)
        {
            if (object.Equals(user,null))
                throw new ArgumentNullException("user");

            _context.Accounts.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(TUser user)
        {
            await _context.AccountDeleteAsync(user.Id);
        }

        public async Task<TUser> FindByIdAsync(Guid userId)
        {
            return (TUser)(await _context.AccountGetAsync(userId));
        }
        public async Task<TUser> FindByEmailAsync(string primaryEmail)
        {
            var user = await _context.AccountGetAsync(primaryEmail, false);

            return (TUser)user;
        }
        public async Task<TUser> FindByEmailAsync(string primaryEmail, string password)
        {
            var user = await _context.AccountGetAsync(primaryEmail, password);

            return (TUser)user;
        }

        public Task<TUser> FindByNameAsync(string userName)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(TUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            _context.Accounts.AddOrUpdate(user);
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public Task<string> GetPasswordHashAsync(TUser user)
        {
            return Task.FromResult<string>(user.Password);
        }
        public Task<bool> HasPasswordAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task SetPasswordHashAsync(TUser user, string passwordHash)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            user.Password = passwordHash;

            return Task.FromResult<object>(null);
        }        

        public Task<string> GetEmailAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetEmailConfirmedAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task SetEmailAsync(TUser user, string email)
        {
            throw new NotImplementedException();
        }

        public Task SetEmailConfirmedAsync(TUser user, bool confirmed)
        {
            return Task.FromResult<object>(null);
        }

        public Task AddClaimAsync(TUser user, Claim claim)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Claim>> GetClaimsAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task RemoveClaimAsync(TUser user, System.Security.Claims.Claim claim)
        {
            throw new NotImplementedException();
        }
    }
}
