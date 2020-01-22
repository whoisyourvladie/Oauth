using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SaaS.Api.Core;
using SaaS.Api.Core.Filters;
using SaaS.Data.Entities.Accounts;
using System;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Web.Http;

namespace SaaS.Api.Controllers.Api.Oauth
{
    public partial class AccountController
    {
        private ObjectCache _cache = MemoryCache.Default;

        private string GetPreferencesMemoryCacheKey()
        {
            return string.Format("account_preferences_{0}", AccountId);
        }
        private void RemovePreferencesMemoryCache()
        {
            _cache.Remove(GetPreferencesMemoryCacheKey());
        }
        private void SetPreferencesMemoryCache(string value)
        {
            _cache.Set(GetPreferencesMemoryCacheKey(), value, DateTimeOffset.UtcNow.AddMinutes(5));
        }
        private string GetPreferencesMemoryCache()
        {
            return _cache.Get(GetPreferencesMemoryCacheKey()) as string;
        }

        private static void CutExtraPreferences(JObject preferences, string[] categories)
        {
            if (object.Equals(categories, null) || categories.Length <= 0)
                return;

            var properties = GetJObjectProperties(preferences);
            foreach (var property in properties)
            {
                if (!categories.Contains(property, StringComparer.InvariantCultureIgnoreCase))
                    preferences.Remove(property);
            }
        }

        [HttpGet, Route("default-preferences")]
        public IHttpActionResult GetDefaultPreferences([FromUri(Name = "category")] string[] categories = null)
        {
            try
            {
                var preferences = DefaultPreferences();

                CutExtraPreferences(preferences, categories);

                return Ok(DefaultPreferences());
            }
            catch (Exception exc) { return Request.HttpExceptionResult(exc); }
        }

        [HttpPost, Route("default-preferences")]
        public async Task<IHttpActionResult> PostDefaultPreferences([FromUri] string key = null)
        {
            if (!"rHQ4TQsQJaHgNsLU".Equals(key, StringComparison.InvariantCultureIgnoreCase))
                return NotFound();

            try
            {
                var json = await Request.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(json))
                {
                    File.WriteAllText(DefaultPreferencesFilePath, json);
                    _defaultPreferencesJson = json;
                }
            }
            catch (Exception exc) { return Request.HttpExceptionResult(exc); }

            return Ok();
        }

        [HttpGet, Route("cache-preferences")]
        public IHttpActionResult GetPreferencesCache()
        {
            try
            {
                return Ok(new { size = System.Runtime.Caching.MemoryCache.Default.GetCount() });
            }
            catch (Exception exc) { return Request.HttpExceptionResult(exc); }
        }

        [HttpGet, Route("preferences"), SaaSAuthorize]
        public async Task<IHttpActionResult> GetPreferences([FromUri(Name = "category")] string[] categories = null)
        {
            try
            {
                var cachePreferences = GetPreferencesMemoryCache();
                var dbPreferences = !string.IsNullOrEmpty(cachePreferences) ?
                    new AccountPreference { Json = cachePreferences } :
                    await _auth.AccountPreferenceGetAsync(AccountId);

                var preferences = !object.Equals(dbPreferences, null) ?
                    JObject.Parse(dbPreferences.Json) :
                    DefaultPreferences();

                SetPreferencesMemoryCache(preferences.ToString(Formatting.None));

                if (!object.Equals(dbPreferences, null))
                {
                    var defaultPreferences = DefaultPreferences();
                    var defaultPreferencesProperties = GetJObjectProperties(defaultPreferences);
                    foreach (var property in defaultPreferencesProperties)
                    {
                        JToken jValue;
                        if (preferences.TryGetValue(property, StringComparison.InvariantCultureIgnoreCase, out jValue))
                            continue;

                        preferences.Add(property, defaultPreferences.GetValue(property));
                    }
                }

                CutExtraPreferences(preferences, categories);

                return Ok(preferences);
            }
            catch (Exception exc) { return Request.HttpExceptionResult(exc); }
        }

        [HttpPost, Route("preferences"), SaaSAuthorize]
        public async Task<IHttpActionResult> PostPreferences([FromUri(Name = "category")] string[] categories = null, bool returnPreferences = false)
        {
            try
            {
                var json = await Request.Content.ReadAsStringAsync();
                var dbPreferences = await _auth.AccountPreferenceGetAsync(AccountId);
                var preferences = object.Equals(dbPreferences, null) ? new JObject() : JObject.Parse(dbPreferences.Json);
                var preferencesJson = JObject.Parse(json);

                foreach (var property in GetJObjectProperties(preferencesJson))
                {
                    JToken jValue;
                    if (preferencesJson.TryGetValue(property, StringComparison.InvariantCultureIgnoreCase, out jValue))
                    {
                        preferences.Remove(property);
                        preferences.Add(property, jValue);
                    }
                }
                json = preferences.ToString(Formatting.None);
                await _auth.AccountPreferenceSetAsync(AccountId, json);

                SetPreferencesMemoryCache(json);

                return returnPreferences ? await GetPreferences(categories) : Ok();
            }
            catch (Exception exc) { return Request.HttpExceptionResult(exc); }
        }

        [HttpDelete, Route("preferences"), SaaSAuthorize]
        public async Task<IHttpActionResult> DeletePreferences([FromUri(Name = "category")] string[] categories = null, bool returnPreferences = false)
        {
            try
            {
                var dbPreferences = await _auth.AccountPreferenceGetAsync(AccountId);
                if (!object.Equals(dbPreferences, null))
                {
                    var preferences = JObject.Parse(dbPreferences.Json);
                    if (!object.Equals(categories, null) && categories.Length > 0)
                    {
                        foreach (var category in categories.Where(category => !string.IsNullOrEmpty(category)))
                            preferences.Remove(category);
                    }
                    else
                        preferences = new JObject();

                    var properties = GetJObjectProperties(preferences);
                    if (properties.Length <= 0)
                        await _auth.AccountPreferenceDeleteAsync(AccountId);
                    else
                    {
                        var json = preferences.ToString(Formatting.None);
                        if (!string.Equals(json, dbPreferences.Json, StringComparison.InvariantCultureIgnoreCase))
                            await _auth.AccountPreferenceSetAsync(AccountId, json);
                    }
                }

                RemovePreferencesMemoryCache();

                return returnPreferences ? await GetPreferences(categories) : Ok();
            }
            catch (Exception exc) { return Request.HttpExceptionResult(exc); }
        }
    }
}