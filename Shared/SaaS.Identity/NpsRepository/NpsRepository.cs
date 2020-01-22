using System;
using System.Threading.Tasks;

namespace SaaS.Identity
{
    public class NpsRepository : INpsRepository
    {
        private readonly AuthDbContext _context;

        public NpsRepository()
        {
            _context = new AuthDbContext();
        }

        public async Task Rate(string questioner, 
            Guid? accountId, 
            string clientName, string clientVersion, 
            byte rating, byte? ratingUsage, string comment)
        {
            await _context.NpsInsertAsync(questioner, 
                accountId, 
                clientName, clientVersion, 
                rating, ratingUsage, comment);
        }

        public void Dispose()
        {
            if (!object.Equals(_context, null))
                _context.Dispose();
        }
    }
}