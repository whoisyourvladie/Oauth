using SaaS.Data.Entities.Accounts;
using System;
using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SaaS.Identity
{
    public partial class AuthRepository
    {
        private static string ToBase64String(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            return Convert.ToBase64String(bytes);
        }
        private static string FromBase64String(string str)
        {
            var bytes = Convert.FromBase64String(str);
            return Encoding.UTF8.GetString(bytes);
        }

        public async Task<Uri> GeneratePasswordResetTokenLinkAsync(Account user, Uri uri)
        {
            string token = await _userManager.GeneratePasswordResetTokenAsync(user.Id);

            var uriBuilder = new UriBuilder(uri);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            query.Add("token", ToBase64String(token));

            query.Add("userId", user.Id.ToString("N"));

            uriBuilder.Query = query.ToString();

            return uriBuilder.Uri;
        }
        public async Task<NameValueCollection> GenerateEmailConfirmationToken(Account user)
        {
            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user.Id);

            var query = HttpUtility.ParseQueryString(string.Empty);

            query.Add("token", ToBase64String(token));
            query.Add("userId", user.Id.ToString("N"));

            return query;
        }
        public async Task<Uri> GenerateEmailConfirmationTokenLinkAsync(Account user, Uri uri)
        {
            var uriBuilder = new UriBuilder(uri);

            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query.Add(await GenerateEmailConfirmationToken(user));
            uriBuilder.Query = query.ToString();

            return uriBuilder.Uri;
        }
    }
}