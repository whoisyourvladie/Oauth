using Microsoft.AspNet.Identity;
using SaaS.Data.Entities.Accounts;
using System.Threading.Tasks;

namespace SaaS.Identity
{
    internal class AccountValidator<TUser> : IIdentityValidator<TUser>
    where TUser : Account
    {
        //private AuthDbContext _context;

        //internal AccountValidator() { }

        //internal AccountValidator(AuthDbContext context)
        //{
        //    _context = context;
        //}

        public async Task<IdentityResult> ValidateAsync(TUser user)
        {
            //if (!object.Equals(_context, null))
            //{
            //    var account = await _context.AccountGetAsync(user.Email, isIncludeSubEmails: true);

            //    if (!object.Equals(account, null) && account.Id != user.Id)
            //        return IdentityResult.Failed("Select a different email address. An account has already been created with this email address.");
            //}

            return await Task.Run(() => {

                return IdentityResult.Success;
            });
        }
    }
}
