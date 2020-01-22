// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IoC.cs" company="Web Advanced">
// Copyright 2012 Web Advanced (www.webadvanced.com)
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


using SaaS.Api.Client;
using SaaS.UI.Admin.App_Start;
using StructureMap;
using StructureMap.Graph;
using System.Configuration;

namespace SaaS.UI.Admin.DependencyResolution
{
    public static class IoC {
        private static string GetString(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public static IContainer Initialize()
        {
            var appSettings = new AppSettings
            {
                OAuth = new AppSettingsOAuth
                {
                    Path = GetString("oauth:path"),
                    ClientId = GetString("oauth:clientId"),
                    ClientSecret = GetString("oauth:clientSecret"),
                    Scope = GetString("oauth:scope")
                },
                Validation = new AppSettingsValidation
                {
                    FirstNamePattern = @"^[^\^<>()\[\]\\;:@?%\|$%*?#&¸¼!+€£¢¾½÷×¤¶»¥¦µ©®°§«´¨™±°º¹²³ª¯¬`""'/]*$",
                    LastNamePattern = @"^[^\^<>()\[\]\\;:@?%\|$%*?#&¸¼!+€£¢¾½÷×¤¶»¥¦µ©®°§«´¨™±°º¹²³ª¯¬`""'/]*$",
                    EmailPattern = @"^[-a-z0-9!#$%&'*+\/=?^_`{|}~]+(\.[-a-z0-9!#$%&'*+\/=?^_`{|}~]+)*@([a-z0-9]([-a-z0-9]{0,61}[a-z0-9])?\.)*[a-z]{1,20}$",
                    UrlPattern = @"^(https?)://[^\s/$.?#].[^\s]*$"
                }
            };

            var oauthService = new SaaSApiOauthService(appSettings.OAuth.Path, appSettings.OAuth.ClientId);

            ObjectFactory.Initialize(x =>
            {
                x.Scan(scan =>
                {
                    scan.TheCallingAssembly();
                    scan.WithDefaultConventions();
                });
                x.For<IAppSettings>().Singleton().Use(appSettings);
                x.For<ISaaSApiOauthService>().Singleton().Use(oauthService);
            });
            return ObjectFactory.Container;
        }
    }
}