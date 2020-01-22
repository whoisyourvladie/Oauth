using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace SaaS.Api.Test.ESign
{
    [TestClass]
    public class Packages : HttpTestHelper
    {
        [TestMethod]
        public async Task GetPackages()
        {
            var json = await AssertIsTrue(await ESignHelper.GetPackages());
        }

        [TestMethod]
        public async Task PostPackages()
        {
            var json = await AssertIsTrue(await ESignHelper.PostPackages());
        }

        [TestMethod]
        public async Task GetPackageId()
        {
            var packageId = new Guid("de5e2862-cf11-4005-a5e6-8c9b1b5f836a");

            var json = await AssertIsTrue(await ESignHelper.GetPackageId(packageId));
        }

        [TestMethod]
        public async Task PostPackageId()
        {
            var packageId = new Guid("c68e0ad5-17e2-4708-91ce-b9112b1c26d8");

            JObject content = new JObject();
            content.Add("trashed", true);

            var json = await AssertIsTrue(await ESignHelper.PostPackageId(packageId, content));
        }

        [TestMethod]
        public async Task DeletePackageId()
        {
            var packageId = new Guid("5538ad7d-c49d-4877-bec0-953dc1c78bed");
            var json = await AssertIsTrue(await ESignHelper.DeletePackageId(packageId));
        }
    }
}
