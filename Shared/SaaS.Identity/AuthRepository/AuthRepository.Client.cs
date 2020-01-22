using SaaS.Data.Entities.Oauth;
using System.Collections.Generic;

namespace SaaS.Identity
{
    public partial class AuthRepository
    {
        public List<Client> ClientsGet()
        {
            return _context.ClientsGet();
        }
    }
}