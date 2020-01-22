using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SaaS.Data.Entities.Accounts;

namespace SaaS.Identity
{
    public partial class AuthRepository
    {
        public async Task SurveyAsync(Guid accountId, byte[] data, string lang)
        {
            await _context.SurveyAsync(accountId, data, lang);
        }

        public async Task<List<AccountSurvey>> GetAllSurveyAsync()
        {
            return await _context.GetAllSurveyAsync();
        }
    }
}
