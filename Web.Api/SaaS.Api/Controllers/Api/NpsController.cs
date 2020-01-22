using SaaS.Api.Core;
using SaaS.Api.Core.Filters;
using SaaS.Api.Models.Nps;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace SaaS.Api.Controllers.Api
{
    [RoutePrefix("api/nps")]
    public class NpsController : SaaSApiController
    {
        [HttpPost, ValidateNullModel, ValidateModel]
        public async Task<IHttpActionResult> Post(NpsViewModel model)
        {
            try
            {
                Guid? accountId = null;
                if (User.Identity.IsAuthenticated)
                    accountId = AccountId;

                await _nps.Rate(model.Questioner,
                    accountId,
                    model.ClientName, model.ClientVersion,
                    model.Rating, model.RatingUsage,
                    model.Comment);

                return Ok();
            }
            catch (Exception exc) { return Request.HttpExceptionResult(exc); }
        }
    }
}