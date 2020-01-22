using SaaS.Api.Core;
using SaaS.Api.Core.Filters;
using SaaS.Common.Extensions;
using SaaS.Data.Entities;
using SaaS.Data.Entities.Oauth;
using SaaS.Data.Entities.View;
using SaaS.IPDetect;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace SaaS.Api.Sign.Controllers.Api.eSign20
{
    [RoutePrefix("api-esign20/v1/presign"), SaaSAuthorize]
    public class ESignPresignController : BaseApiController
    {
        protected override string ApiRoot
        {
            get { return "api/v1/signers/"; }
        }
        [Route, HttpPost]
        public async Task<HttpResponseMessage> Presign(CancellationToken cancellationToken)
        {
            try
            {
                var session = await GetSessionToken();
                if (object.Equals(session, null))
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);

                if ("esign-lite".Equals(session.ClientName, StringComparison.InvariantCultureIgnoreCase))
                    return await PresignESignLite(session, cancellationToken);

                return Request.CreateExceptionResponse(errorDescription: "Bad client_id.", statusCode: HttpStatusCode.BadRequest);
            }
            catch (Exception exc) { return Request.CreateExceptionResponse(exc); }
        }

        private async Task<HttpResponseMessage> PresignESignLite(SessionToken session, CancellationToken cancellationToken)
        {
            int? allowed = null;
            int? used = null;

            ViewAccountMicrotransaction microtransaction;
            var microtransactions = await _authProduct.AccountMicrotransactionsGetAsync(session.AccountId);
            var accountMicrotransactionId = eSignHelper.GetAllowedAccountMicrotransactionId(microtransactions, out microtransaction);

            if (accountMicrotransactionId.HasValue && !microtransaction.AllowedEsignCount.HasValue)
                return IncludeHeaders(Request.CreateResponse(), microtransaction.AllowedEsignCount, microtransaction.UsedEsignCount);

            /*--------------------------------------------------------------------------------*/

            ViewAccountProduct product;
            var products = await _authProduct.AccountProductsGetAsync(session.AccountId);
            var accountProductId = eSignHelper.GetAllowedAccountProductId(products, session.AccountProductId, out product);

            if (accountProductId.HasValue && !product.AllowedEsignCount.HasValue)
                return IncludeHeaders(Request.CreateResponse(), product.AllowedEsignCount, product.UsedEsignCount);

            /*--------------------------------------------------------------------------------*/
            
            var type = eSignTransactionInitiatorType.Free;
            if (accountMicrotransactionId.HasValue && microtransaction.AllowedEsignCount.HasValue)
            {
                eSignHelper.GetAllowedUsedCount(products, microtransactions, out allowed, out used);
                type = eSignTransactionInitiatorType.Microtransaction;
            }
            else
            {
                if (accountProductId.HasValue && product.AllowedEsignCount.HasValue)
                {
                    eSignHelper.GetAllowedUsedCount(products, microtransactions, out allowed, out used);
                    type = eSignTransactionInitiatorType.Product;
                }
            }

            var json = await Request.Content.ReadAsStringAsync();
            var ipAddressHash = IpAddressDetector.IpAddress.GetShortMD5Hash();
            var freeNumberOfSigns = await _eSign.eSignPackageHistoryGetFreeNumberOfSigns(session.ClientId, eSignClient, ipAddressHash);

            var fileSize = GetFileContentLength();
            var recipients = GetRecipientsCount(json);

            if (type == eSignTransactionInitiatorType.Free || type == eSignTransactionInitiatorType.Microtransaction)
            {
                if (fileSize > eSignLiteLimitation.FileSize)
                    return PaymentRequired(type == eSignTransactionInitiatorType.Microtransaction ?
                        eSignLiteLimitation.FileSizeMinorWarning :
                        eSignLiteLimitation.FileSizeError, allowed, used);

                if (recipients > eSignLiteLimitation.Recipients)
                    return PaymentRequired(type == eSignTransactionInitiatorType.Microtransaction ?
                     eSignLiteLimitation.RecipientsMinorWarning :
                     eSignLiteLimitation.RecipientsError, allowed, used);

                if (freeNumberOfSigns >= eSignLiteLimitation.SignsPerDay || type == eSignTransactionInitiatorType.Microtransaction)
                    return PaymentRequired(type == eSignTransactionInitiatorType.Microtransaction ?
                        eSignLiteLimitation.SignsPerDayMinorWarning :
                        eSignLiteLimitation.SignsPerDayError, allowed, used);
            }

            if (type == eSignTransactionInitiatorType.Product)
            {
                if (fileSize > eSignLiteLimitation.FileSize)
                    return PaymentRequired(eSignLiteLimitation.FileSizeMinorWarning, allowed, used);

                if (recipients > eSignLiteLimitation.Recipients)
                    return PaymentRequired(eSignLiteLimitation.RecipientsMinorWarning, allowed, used);

                if (freeNumberOfSigns >= eSignLiteLimitation.SignsPerDay)
                    return PaymentRequired(eSignLiteLimitation.SignsPerDayMinorWarning, allowed, used);
            }

            return Request.CreateResponse();
        }
    }
}
