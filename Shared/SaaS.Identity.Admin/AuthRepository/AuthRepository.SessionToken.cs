using SaaS.Data.Entities.Admin.Oauth;
using System;
using System.Threading.Tasks;

namespace SaaS.Identity.Admin
{
    public partial class AuthRepository
    {
        public async Task<SessionToken> SessionTokenGetAsync(Guid id)
        {
            return await _context.SessionTokenGetAsync(id);
        }
        public async Task SessionTokenInsertAsync(SessionToken token)
        {
            await _context.SessionTokenInsertAsync(token);
        }
    }
}