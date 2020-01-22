using System;
using System.Web.Http;
using WebApi.OutputCache.V2;

namespace SaaS.Api.Controllers.Api
{
    [RoutePrefix("api/config")]
    public class ConfigController : ApiController
    {
        [HttpGet]
        [CacheOutput(ClientTimeSpan = 864000, ServerTimeSpan = 864000)]
        public IHttpActionResult Config()
        {
            return Ok(new
            {
                minRequestDelay = (ulong)TimeSpan.FromMinutes(1).TotalSeconds,
                clientCacheTimeLive = (ulong)TimeSpan.FromDays(7).TotalSeconds
            });
        }
    }
}