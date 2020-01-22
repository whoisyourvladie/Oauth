using System;
using System.Collections.Specialized;
using System.IO;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Upclick.Api.Client
{
    internal class UpclickHttpClient
    {
        private string _uid, _password;

        public UpclickHttpClient(string uid, string password)
        {
            _uid = uid;
            _password = password;
        }

        private static HttpClient CreateHttpClient()
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("https://rest.upclick.com");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }

        internal async Task<HttpResponseMessage> GetAsync(string requestUri, NameValueCollection query = null)
        {
            string token = Token(requestUri.Replace('/', '.'));
            string uri = Path.Combine(token, requestUri);

            if (!object.Equals(query, null))
            {
                var values = HttpUtility.ParseQueryString(string.Empty);
                values.Add(query);

                uri = string.Format("{0}?{1}", uri, values);
            }
            
            using (var client = CreateHttpClient())
                return await client.GetAsync(uri);
        }

        internal async Task<HttpResponseMessage> PostAsync<T>(string requestUri, T value, NameValueCollection query = null)
        {
            string token = Token(requestUri.Replace('/', '.'));
            string uri = Path.Combine(token, requestUri);

            using (var client = CreateHttpClient())
                return await client.PostAsync(uri, value, new JsonMediaTypeFormatter());
        }

        private string MD5Hash(string str)
        {
            Encoder encoder = Encoding.Unicode.GetEncoder();
            byte[] bytes = new byte[str.Length * 2];
            encoder.GetBytes(str.ToCharArray(), 0, str.Length, bytes, 0, true);

            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] result = md5.ComputeHash(bytes);

            StringBuilder builder = new StringBuilder();

            foreach (var item in result)
                builder.Append(item.ToString("X2"));

            return builder.ToString();
        }

        public string Token(string method)
        {
            if (!string.IsNullOrEmpty(method))
                method = method.ToLower();

            string password = MD5Hash(_password);
            string timeStamp = DateTime.UtcNow.AddMinutes(-5).ToString("yyyy-MM-dd HH:mm:ss.fff");
            string token = string.Join("|", _uid, timeStamp, MD5Hash(string.Concat(password, timeStamp, method)));

            byte[] bytes = Encoding.UTF8.GetBytes(token);
            return Convert.ToBase64String(bytes);
        }
    }
}
