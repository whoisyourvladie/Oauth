using CsvHelper;
using SaaS.Data.Entities.Admin.Oauth;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;

namespace SaaS.Api.Admin.Controllers.Api
{
    [RoutePrefix("api/log"), Authorize(Roles = "admin")]
    public class LogController : SaaSApiController
    {

        [HttpGet, AllowAnonymous]
        public async Task<IHttpActionResult> Index(DateTime from, DateTime to, Guid? userId = null, string log = null, LogActionTypeEnum? logActionTypeId = null,
            string format = "json")
        {
            var logs = await _authAdmin.LogsGetAsync(from, to, userId, log, logActionTypeId);

            if ("csv".Equals(format, StringComparison.InvariantCultureIgnoreCase))
            {
                var memoryStream = new MemoryStream();
                var streamWriter = new StreamWriter(memoryStream);
                var csvWriter = new CsvWriter(streamWriter);

                csvWriter.WriteRecords(logs);
                memoryStream.Position = 0;

                var response = new HttpResponseMessage(HttpStatusCode.OK);

                response.Content = new StreamContent(memoryStream);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/csv");
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = string.Format("logs-from {0} to {1}.csv", from.ToString("MMMM dd, yyyy"), to.ToString("MMMM dd, yyyy"))
                };

                return ResponseMessage(response);
            }

            return Ok(logs);
        }

        [HttpGet, Route("action-type")]
        public async Task<IHttpActionResult> ActionType()
        {
            return Ok(await _authAdmin.LogActionTypesGetAsync());
        }
    }
}