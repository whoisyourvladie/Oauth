using SaaS.Data.Entities.Oauth;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SaaS.Identity
{
    public partial class AuthRepository
    {
        public async Task<SessionToken> SessionTokenGetAsync(Guid id)
        {
            return await _context.SessionTokenGetAsync(id);
        }
        public async Task<List<SessionToken>> SessionTokensGetAsync(Guid accountId)
        {
            return await _context.SessionTokensGetAsync(accountId);
        }
        public async Task SessionTokenDeleteAsync(Guid id)
        {
            await _context.SessionTokenDeleteAsync(id);
        }
        public async Task SessionTokenInsertAsync(SessionToken token, Guid? oldId, bool isRemoveOldSessions, bool isInsertAccountSystem)
        {
            await _context.SessionTokenInsertAsync(token, oldId, isRemoveOldSessions, isInsertAccountSystem);
        }
    }
}