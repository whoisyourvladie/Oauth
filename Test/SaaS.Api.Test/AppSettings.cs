using System;

namespace SaaS.Api.Test
{
    public sealed class AppSettings : IAppSettings
    {
        public Uri PathToOAuthApi { get; private set; }
        public Uri PathToESignApi { get; private set; }

        public string ClientId { get; private set; }
        public string ClientSecret { get; private set; }
        public string Scope { get; private set; }

        public string Login { get; private set; }
        public string Password { get; private set; }
        public string Token { get; private set; }



        public AppSettings()
        {
            PathToOAuthApi = new Uri("https://stage-oauth.sodapdf.com");            
            //PathToOAuthApi = new Uri("https://oauth-dev.sodapdf.com");
            //PathToESignApi = new Uri("https://oauth-sign-dev.sodapdf.com");

#if DEBUG
            PathToOAuthApi = new Uri("http://localhost:52289");
            PathToESignApi = new Uri("http://localhost:53583");
#endif

            #region Login/Password

            //Login = "jdobrofsky@lulusoftware.com";
            //Password = "password";

            //Login = "abalyuk @lulusoftware.com.ua";
            //Password = "Windows!7";

            //Login = "asuleymanov@lulusoftware.com";
            //Password = "anatole64";
            //Token = "YQPJDG43BZMTX0YB33ZX3XEGJYH8BM0F";

            Login = "azverev@lulusoftware.com";
            Password = "anatole64";
            Token = "YQPJDG43BZMTX0YB33ZX3XEGJYH8BM0F";

            //Login = "sgrishchenko@lulusoftware.com";
            //Password = "anatole64";
            //Token = "YQPJDG43BZMTX0YB33ZX3XEGJYH8BM0F";

            //Login = "uasergbox@gmail.com";
            //Password = "Qazqazqaz1";
            //Token = "YQPJDG43BZMTX0YB33ZX3XEGJYH8BM0F";

            //Login = "hail2002@mail.ru";
            //Password = "qwerty";
            //Token = "YQPJDG43BZMTX0YB33ZX3XEGJYH8BM0F";

            //Login = "knedilko@lulusoftware.com";
            //Password = "Azerty12";
            //Token = "YQPJDG43BZMTX0YB33ZX3XEGJYH8BM0F";

            //Login = "kirill4@mailinator.com";
            //Password = "Azerty12";
            //Token = "YQPJDG43BZMTX0YB33ZX3XEGJYH8BM0F";

            //Login = "kirill4@mailinator.com";
            //Password = "Azerty12";

            //Login = "sometest@lulusoftware.com";
            //Password = "111111";
            //Token = "77E22FBW4Z4A7C7V3S2HPQ91DPXEV6VX";

            //Login = "ozvieriev@gmail.com";
            //Password = "putinhuylo123!";
            //Token = "T32QT6D2NZW99HPS73E7NM4AM0T9KRTP";

            //Login = "lulu-ult-2016-06@ctdeveloping.com";
            //Password = "ianbacon";
            //Token = "C57D64KH6K276CVC098BNS4QPQ84QQDW";


            //Login = "alexey.vlasenko@outlook.com";
            //Password = "vBHUQCJO5m";
            //Token = "ZE78A5ZRPEJF7CNKMS18WH3QA1RJDCGK";

            //Login = "alex-rv2007@mail.ru";
            //Password = "123456";

            //Login = "alysenko@lulusoftware.com";
            //Password = "anatole64";

            #endregion

            //ClientId = "desktop";
            //ClientSecret = "mfjmibwJ2ILGFPVV/S95K4hHzTD4ZBxV/bDS/TUKtw=";

            //Scope = "editor";

            ClientId = "saas";
            ClientSecret = "";

            Scope = "webeditor";
        }
    }

    public interface IAppSettings
    {
        Uri PathToOAuthApi { get; }
        Uri PathToESignApi { get; }
    }
}
