using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SaaS.Identity.Admin
{
    public class UserValidator<TUser> : IIdentityValidator<TUser>
    where TUser : User
    {
        private readonly UserManager<TUser, Guid> _manager;

        public UserValidator() { }

        public UserValidator(UserManager<TUser, Guid> manager)
        {
            _manager = manager;
        }

        public async Task<IdentityResult> ValidateAsync(TUser item)
        {
            var errors = new List<string>();

            if (_manager != null)
            {
                var otherAccount = await _manager.FindByEmailAsync(item.Login);
                if (otherAccount != null && otherAccount.Id != item.Id)
                    errors.Add("Select a different email address. An account has already been created with this email address.");
            }

            return errors.Any()
                ? IdentityResult.Failed(errors.ToArray())
                : IdentityResult.Success;
        }
    }
}
