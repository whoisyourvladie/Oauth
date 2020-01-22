using SaaS.Data.Entities.Admin.Oauth;
using SaaS.Data.Entities.Admin.View;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SaaS.Identity.Admin
{
    public partial class AuthRepository
    {
        public async Task<List<ViewLog>> LogsGetAsync(DateTime from, DateTime to, Guid? userId, string log, LogActionTypeEnum? logActionType)
        {
            return await _context.LogsGetAsync(from, to, userId, log, logActionType);
        }
        public async Task LogInsertAsync(Guid userId, Guid? accountId, string log, LogActionTypeEnum logActionType)
        {
            await _context.LogInsertAsync(userId, accountId, log, logActionType);
        }

        public async Task<List<LogActionType>> LogActionTypesGetAsync()
        {
            return await _context.LogActionTypesGetAsync();
        }
    }
}