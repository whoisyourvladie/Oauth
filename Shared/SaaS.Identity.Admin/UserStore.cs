using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SaaS.Identity.Admin
{
    public class UserStore<TUser> : IUserStore<TUser, Guid>, IUserEmailStore<TUser, Guid>, IUserPasswordStore<TUser, Guid>, IUserClaimStore<TUser, Guid>
        where TUser : User
    {
        private readonly AuthDbContext _context;

        public UserStore(AuthDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(TUser user)
        {
            if (object.Equals(user, null))
                throw new ArgumentNullException("user");

            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
            catch
            {
                throw;
            }
        }

        public Task DeleteAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public async Task<TUser> FindByIdAsync(Guid id)
        {
            return (TUser)(await _context.UserGetAsync(id));
        }
        public async Task<TUser> FindByEmailAsync(string email)
        {
            return (TUser)(await _context.UserGetAsync(email));
        }
        public Task<TUser> FindByEmailAsync(string email, string password)
        {
            throw new NotImplementedException();
        }

        public async Task<TUser> FindByNameAsync(string userName)
        {
            return (TUser)(await _context.UserGetAsync(userName));
        }

        public async Task UpdateAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            try
            {
                _context.Users.AddOrUpdate(user);
                await _context.SaveChangesAsync();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
            catch (Exception) { throw; }
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
