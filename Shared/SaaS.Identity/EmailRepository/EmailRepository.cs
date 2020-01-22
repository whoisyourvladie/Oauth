using SaaS.Data.Entities;
using SaaS.Data.Entities.Accounts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SaaS.Identity
{
    public class EmailRepository : IEmailRepository
    {
        private readonly AuthDbContext _context;

        public EmailRepository()
        {
            _context = new AuthDbContext();
        }

        public async Task EmailInsertAsync(Guid accountId, Status status, string emailCustomParam, string modelCustomParam)
        {
            await _context.EmailInsertAsync(accountId, status, emailCustomParam, modelCustomParam);
        }
        public async Task<List<Email>> EmailsGetAsync(Status status = Status.NotStarted, int top = 10)
        {
            return await _context.EmailsGetAsync(status, top);
        }
        public async Task EmailStatusSetAsync(int id, Status status)
        {
            await _context.EmailStatusSetAsync(id, status);
        }

        public void Dispose()
        {
            if (!object.Equals(_context, null))
                _context.Dispose();
        }
    }
}