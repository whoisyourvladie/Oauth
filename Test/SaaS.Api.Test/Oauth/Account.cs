using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SaaS.Api.Test.Models.Api.Oauth;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SaaS.Api.Test.Oauth
{
    [TestClass]
    public class Account : HttpTestHelper
    {

        [TestMethod]
        public async Task TEST()
        {
            var token = await OAuthHelper.RegisterAnonymous();


            //var json = await AssertIsTrue(await OAuthHelper.Auto(token.access_token));

            //products = await OAuthHelper.GetProduct(new Guid("5d3dda08-0946-4107-97d0-188d86c9959f"), token.access_token);
            //json = await AssertIsTrue(products);
        }


        #region Register

        [TestMethod]
        public async Task Register()
        {
            var model = OAuthHelper.RegisterViewModel();
            Debug.WriteLine("Trying register user...");

            var json = await AssertIsTrue(await OAuthHelper.Register(model));
        }

        [TestMethod]
        public async Task RegisterUnique()
        {
            var model = OAuthHelper.CreateRandomRegisterViewModel();
            Debug.WriteLine("Trying register unique user...");

            var json = await AssertIsTrue(await OAuthHelper.Register(model));
        }

        [TestMethod]
        public async Task RegisterAnonymous()
        {
            Debug.WriteLine("Trying register anonymous user ...");

            var json = await AssertIsTrue(await OAuthHelper.RegisterAnonymous());
        }

        [TestMethod]
        public async Task RegisterNotUnique()
        {
            var model = OAuthHelper.CreateRandomRegisterViewModel();
            Debug.WriteLine("Trying register not unique user...");

            await OAuthHelper.Register(model);

            var json = await AssertIsFalse(await OAuthHelper.Register(model));
        }

        [TestMethod]
        public async Task RegisterWithNullModel()
        {
            Debug.WriteLine("Trying register with null module...");

            var json = await AssertIsFalse(await OAuthHelper.Register(null));
        }

        [TestMethod]
        public async Task RegisterWithWrongEmail()
        {
            var model = OAuthHelper.CreateRandomRegisterViewModel();
            model.Email = "hello";

            Debug.WriteLine("Trying register unique user with wrong email...");

            var json = await AssertIsFalse(await OAuthHelper.Register(model));
        }

        [TestMethod]
        public async Task RegisterWithEmptyEmail()
        {
            var model = OAuthHelper.CreateRandomRegisterViewModel();
            model.Email = null;

            Debug.WriteLine("Trying register unique user with empty email...");

            var json = await AssertIsFalse(await OAuthHelper.Register(model));
        }

        [TestMethod]
        public async Task RegisterWithEmptyPassword()
        {
            var model = OAuthHelper.CreateRandomRegisterViewModel();
            model.Password = null;

            Debug.WriteLine("Trying register unique user with empty password...");

            var json = await AssertIsFalse(await OAuthHelper.Register(model));
        }

        #endregion

        #region SignIn

        [TestMethod]
        public async Task SignIn()
        {
            var token = await OAuthHelper.SignIn();
            //var token2 = await OAuthHelper.SignIn();

            var response = await OAuthHelper.RefreshToken(token.refresh_token);
        }

        [TestMethod]
        public async Task Logout()
        {
            var token = await OAuthHelper.Logout();


        }

        [TestMethod]
        public async Task ReSignIn()
        {
            var token = await OAuthHelper.ReSignIn();

            await OAuthHelper.RefreshToken(token.refresh_token);
        }

        #endregion

        #region SendActivationEmail

        [TestMethod]
        public async Task SendActivationEmail()
        {
            var model = OAuthHelper.RegisterViewModel();
            Debug.WriteLine("Trying to send activation email...");

            var json = await AssertIsTrue(await OAuthHelper.SendActivationEmail(model));
        }

        [TestMethod]
        public async Task SendActivationEmailWithNullModel()
        {
            Debug.WriteLine("Trying to send activation email with null module...");

            var json = await AssertIsFalse(await OAuthHelper.SendActivationEmail(null));
        }

        [TestMethod]
        public async Task SendActivationEmailWithEmptyEmail()
        {
            var model = new AuthViewModel();
            Debug.WriteLine("Trying to send activation email with empty email...");

            var json = await AssertIsFalse(await OAuthHelper.SendActivationEmail(model));
        }

        #endregion

        #region Products

        [TestMethod]
        public async Task Products()
        {
            var json = await AssertIsTrue(await OAuthHelper.GetProduct(new Guid("14732597-54c2-42e1-8390-5aa86387e1af")));

            var access_token = JObject.Parse(json).GetValue("access_token").Value<string>();

            var d  = await OAuthHelper.GetModules(access_token );
            json = await d.Content.ReadAsStringAsync();
        }

        #endregion

        #region ChangePassword

        //[TestMethod]
        //public async Task ChangePassword()
        //{
        //    var changePasswordModel = new ChangePasswordViewModel()
        //    {
        //        OldPassword = "123456",
        //        NewPassword = "123456!!!"
        //    };

        //    var json = await AssertIsTrue(await OAuthHelper.ChangePassword(changePasswordModel));
        //}

        #endregion

        //#region RecoverPassword

        //private static async Task<HttpResponseMessage> RecoverPassword(AuthViewModel model)
        //{
        //    using (var client = CreateHttpClient())
        //    {
        //        var response = await client.PostAsJsonAsync<AuthViewModel>("api/oauth/account/recover-password", model);

        //        Debug.WriteLine("Response status code: {0}", response.StatusCode);

        //        return response;
        //    }
        //}

        //[TestMethod]
        //public async Task RecoverPassword()
        //{
        //    var model = await CreateRegisterViewModel();

        //    Debug.WriteLine("Trying to recover password in user...");
        //    AssertIsTrue(await RecoverPassword(model));

        //    Debug.WriteLine("Trying sign in user with old password...");
        //    AssertIsFalse(await SignIn(model.Email, model.Password));
        //}

        ////[TestMethod]
        ////public async Task RecoverPasswordForNormalUser()
        ////{
        ////    var model = new RegisterViewModel()
        ////    {
        ////        Email = "azverev@lulusoftware.com",
        ////        Password = "qwerty123",
        ////        ConfirmPassword = "qwerty123",
        ////        FirstName = "alex",
        ////        LastName = "zverev"
        ////    };

        ////    var response = await Register(model);

        ////    Debug.WriteLine("Trying to recover password in user...");
        ////    response = await RecoverPassword(model);
        ////    Assert.IsTrue(response.IsSuccessStatusCode);

        ////    Debug.WriteLine("Trying sign in user with old password...");
        ////    response = await SignIn(model.Email, model.Password);
        ////    Assert.IsFalse(response.IsSuccessStatusCode);
        ////}

        //#endregion


        //#endregion

        //#region Info

        //[TestMethod]
        //public async Task GetAccountInfoForNormalUser()
        //{
        //    TokenResultModel token = await SignInForNormalUser();

        //    using (var client = CreateHttpClient())
        //    {
        //        client.DefaultRequestHeaders.Add("Authorization", string.Format("{0} {1}", token.token_type, token.access_token));
        //        AssertIsTrue(await client.GetAsync("api/oauth/account/info"));
        //    }
        //}

        //[TestMethod]
        //public async Task GetAccountInfoNotAuthorized()
        //{
        //    using (var client = CreateHttpClient())
        //        AssertIsFalse(await client.GetAsync("api/oauth/account/info"));
        //}

        //[TestMethod]
        //public async Task SetAccountInfoForNormalUser()
        //{
        //    TokenResultModel token = await SignInForNormalUser();

        //    var changeInfoViewModel = new ChangeInfoViewModel()
        //    {
        //        FirstName = string.Format("FirstName - {0}", Guid.NewGuid()),
        //        LastName = string.Format("LastName - {0}", Guid.NewGuid())
        //    };
        //    using (var client = CreateHttpClient())
        //    {
        //        client.DefaultRequestHeaders.Add("Authorization", string.Format("{0} {1}", token.token_type, token.access_token));
        //        AssertIsTrue(await client.PostAsJsonAsync<ChangeInfoViewModel>("api/oauth/account/info", changeInfoViewModel));
        //    }

        //    token = await SignInForNormalUser();

        //    Assert.IsTrue(changeInfoViewModel.FirstName == token.firstName);
        //    Assert.IsTrue(changeInfoViewModel.LastName == token.lastName);
        //}

        //[TestMethod]
        //public async Task SetAccountInfoNotAuthorized()
        //{
        //    var changeInfoViewModel = new ChangeInfoViewModel()
        //    {
        //        FirstName = string.Format("FirstName - {0}", Guid.NewGuid()),
        //        LastName = string.Format("LastName - {0}", Guid.NewGuid())
        //    };

        //    using (var client = CreateHttpClient())
        //        AssertIsFalse(await client.PostAsJsonAsync<ChangeInfoViewModel>("api/oauth/account/info", changeInfoViewModel));
        //}

        //[TestMethod]
        //public async Task SetAccountInfoForNormalUserWithNullModel()
        //{
        //    TokenResultModel token = await SignInForNormalUser();

        //    using (var client = CreateHttpClient())
        //    {
        //        client.DefaultRequestHeaders.Add("Authorization", string.Format("{0} {1}", token.token_type, token.access_token));
        //        AssertIsFalse(await client.PostAsJsonAsync<ChangeInfoViewModel>("api/oauth/account/info", null));
        //    }
        //}

        //[TestMethod]
        //public async Task SetAccountInfoForNormalUserWithEmptyFirstNameAndLastName()
        //{
        //    TokenResultModel token = await SignInForNormalUser();

        //    var changeInfoViewModel = new ChangeInfoViewModel() { };
        //    using (var client = CreateHttpClient())
        //    {
        //        client.DefaultRequestHeaders.Add("Authorization", string.Format("{0} {1}", token.token_type, token.access_token));
        //        AssertIsFalse(await client.PostAsJsonAsync<ChangeInfoViewModel>("api/oauth/account/info", changeInfoViewModel));
        //    }
        //}

        //#endregion

        //#region Data

        //[TestMethod]
        //public async Task GetAccountDataRecentDocumentsForNormalUser()
        //{
        //    TokenResultModel token = await SignInForNormalUser();

        //    using (var client = CreateHttpClient())
        //    {
        //        client.DefaultRequestHeaders.Add("Authorization", string.Format("{0} {1}", token.token_type, token.access_token));
        //        AssertIsTrue(await client.GetAsync("api/oauth/account/data/recent-documents"));
        //    }
        //}

        //[TestMethod]
        //public async Task GetAccountDataRecentDocumentsNotAuthorized()
        //{
        //    using (var client = CreateHttpClient())
        //        AssertIsFalse(await client.GetAsync("api/oauth/account/data/recent-documents"));
        //}

        //#endregion

        //#region Settings

        //[TestMethod]
        //public async Task GetAccountSettingsForNormalUser()
        //{
        //    TokenResultModel token = await SignInForNormalUser();

        //    using (var client = CreateHttpClient())
        //    {
        //        client.DefaultRequestHeaders.Add("Authorization", string.Format("{0} {1}", token.token_type, token.access_token));
        //        AssertIsTrue(await client.GetAsync("api/account/settings"));
        //    }
        //}

        //[TestMethod]
        //public async Task GetAccountSettingsNotAuthorized()
        //{
        //    using (var client = CreateHttpClient())
        //        AssertIsFalse(await client.GetAsync("api/account/settings"));
        //}

        //#endregion
    }
}