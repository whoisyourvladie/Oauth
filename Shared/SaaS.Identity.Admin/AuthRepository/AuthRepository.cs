using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.DataProtection;
using SaaS.Common.Extensions;
using System;

namespace SaaS.Identity.Admin
{
    public partial class AuthRepository : IAuthRepository
    {
        protected readonly AuthDbContext _context;
        protected readonly UserManager<User, Guid> _userManager;

        public AuthRepository()
        {
            _context = new AuthDbContext();

            _userManager = new UserManager<User, Guid>(new UserStore<User>(_context));
            _userManager.UserValidator = new UserValidator<User>(_userManager);
            _userManager.PasswordHasher = new SaaSPasswordHasher();
        }

        public AuthRepository(string connectionString)
        {
            _context = new AuthDbContext(connectionString);

            _userManager = new UserManager<User, Guid>(new UserStore<User>(_context));
            _userManager.UserValidator = new UserValidator<User>(_userManager);
            _userManager.PasswordHasher = new SaaSPasswordHasher();
        }


        public void SetDataProtectorProvider(IDataProtectionProvider dataProtectorProvider)
        {
            var dataProtector = dataProtectorProvider.Create("SodaPDFAdmin");
            _userManager.UserTokenProvider = new DataProtectorTokenProvider<User, Guid>(dataProtector)
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