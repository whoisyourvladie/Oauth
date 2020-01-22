using SaaS.Data.Entities;
using SaaS.Data.Entities.Accounts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SaaS.Identity
{
    public interface IEmailRepository : IDisposable
    {
        Task EmailInsertAsync(Guid accountId, Status status, string emailCustomParam, string modelCustomParam);
        Task<List<Email>> EmailsGetAsync(Status status, int top);
        Task EmailStatusSetAsync(int id, Status status);
    }
}
