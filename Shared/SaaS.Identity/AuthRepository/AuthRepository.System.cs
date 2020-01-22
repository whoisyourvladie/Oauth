using System.Threading.Tasks;

namespace SaaS.Identity
{
    public partial class AuthRepository
    {
        public async Task<Data.Entities.Oauth.OauthSystem> SystemInsertAsync(Data.Entities.Oauth.OauthSystem system)
        {
            return await _context.SystemInsertAsync(system);
        }
    }
}