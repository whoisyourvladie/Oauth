using SaaS.Data.Entities.Oauth;
using System;
using System.Threading.Tasks;

namespace SaaS.Identity
{
    public interface INpsRepository : IDisposable
    {
        Task Rate(string questioner, 
            Guid? accountId, 
            string clientName, string clientVersion, 
            byte rating, byte? ratingUsage, string comment);
    }
}