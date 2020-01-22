using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace SaaS.Api.Test
{
    public class HttpTestHelper
    {
        protected static async Task<string> AssertIsTrue(HttpResponseMessage response)
        {
            Debug.WriteLine("Response status code: {0}", response.StatusCode);
            string json = await response.Content.ReadAsStringAsync();

            Assert.IsTrue(response.IsSuccessStatusCode);

            return json;
        }

        protected static async Task<string> AssertIsFalse(HttpResponseMessage response)
        {
            Debug.WriteLine("Response status code: {0}", response.StatusCode);
            string json = await response.Content.ReadAsStringAsync();

            Assert.IsFalse(response.IsSuccessStatusCode);

            return json;
        }
    }
}
