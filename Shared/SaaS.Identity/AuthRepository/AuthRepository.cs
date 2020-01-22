using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.DataProtection;
using SaaS.Common.Extensions;
using SaaS.Data.Entities.Accounts;
using System;

namespace SaaS.Identity
{
    public partial class AuthRepository : IAuthRepository
    {
        protected readonly AuthDbContext _context;
        protected readonly UserManager<Account, Guid> _userManager;

        public AuthRepository()
        {
            _context = new AuthDbContext();

            _userManager = new UserManager<Account, Guid>(new AccountStore<Account>(_context));
            _userManager.UserValidator = new AccountValidator<Account>();
            _userManager.PasswordHasher = new SaaSPasswordHasher();
        }

        public void SetDataProtectorProvider(IDataProtectionProvider dataProtectorProvider)
        {
            var dataProtector = dataProtectorProvider.Create("SodaPDF");
            _userManager.UserTokenProvider = new DataProtectorTokenProvider<Account, Guid>(dataProtector)
            {
                TokenLifespan = TimeSpan.FromHours(24),
            };
        }

        public void Dispose()
        {
            _context.Dispose();
            _userManager.Dispose();
        }
    }
}