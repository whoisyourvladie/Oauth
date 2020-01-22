using Newtonsoft.Json.Linq;
using SaaS.Api.Core.Filters;
using SaaS.Common.Extensions;
using SaaS.Data.Entities.eSign;
using SaaS.Data.Entities.Oauth;
using SaaS.Data.Entities.View;
using SaaS.IPDetect;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace SaaS.Api.Sign.Controllers.Api.eSign20
{
    [RoutePrefix("api-esign20/v1/packages"), SaaSAuthorize]
    public class ESignPackagesController : BaseApiController
    {
        protected override string ApiRoot
        {
            get { return "api/v1/packages/"; }
        }

        [Route("{*url}"), HttpGet, HttpPost, HttpPut, HttpDelete]
        public async Task<HttpResponseMessage> Index(CancellationToken cancellationToken)
        {
            return await HttpProxy(Request, Request.RequestUri.LocalPath, cancellationToken);
        }

        [Route, HttpGet, HttpPost]
        public async Task<HttpResponseMessage> Packages(CancellationToken cancellationToken)
        {
            return await HttpProxy(Request, Format(), cancellationToken);
        }

        private async Task<HttpResponseMessage> Send(string json, Guid packageId, CancellationToken cancellationToken)
        {
            var session = await GetSessionToken();
            if (object.Equals(session, null))
                return Request.CreateResponse(HttpStatusCode.Unauthorized);

            Guid? accountProductId = null;
            Guid? accountMicrotransactionId = null;
            if (!await IsAvailableFreeSigns(json, session))
            {
                ViewAccountMicrotransaction microtransaction;
                var microtransactions = await _authProduct.AccountMicrotransactionsGetAsync(session.AccountId);
                accountMicrotransactionId = eSignHelper.GetAllowedAccountMicrotransactionId(microtransactions, out microtransaction);

                ViewAccountProduct product;
                var products = await _authProduct.AccountProductsGetAsync(session.AccountId);
                accountProductId = eSignHelper.GetAllowedAccountProductId(products, session.AccountProductId, out product);

                if (!accountMicrotransactionId.HasValue && !accountProductId.HasValue)
                    return PaymentRequired("User doesn't have enough number of signs.");
                else
                {
                    if (accountMicrotransactionId.HasValue && !microtransaction.AllowedEsignCount.HasValue)
                        accountProductId = null;

                    if (accountProductId.HasValue && !product.AllowedEsignCount.HasValue)
                        accountMicrotransactionId = null;

                    if (accountMicrotransactionId.HasValue && accountProductId.HasValue)
                        accountProductId = null;
                }
            }

            var response = await HttpProxy(Request, Format("{0}", packageId), cancellationToken);
            if (response.IsSuccessStatusCode && (accountProductId.HasValue || accountMicrotransactionId.HasValue))
                await _eSign.eSignUseIncreaseAsync(session.AccountId, accountProductId, accountMicrotransactionId);

            var history = new eSignPackageHistory
            {
                AccountId = session.AccountId,
                oAuthClientId = session.ClientId,
                eSignClientId = eSignClient,
                HttpStatusCode = (int)response.StatusCode,
                IsSuccess = response.IsSuccessStatusCode,
                PackageId = packageId,
                IpAddressHash = IpAddressDetector.IpAddress.GetShortMD5Hash(),

                AccountProductId = accountProductId,
                AccountMicrotransactionId = accountMicrotransactionId
            };

            await _eSign.eSignPackageHistorySetAsync(history);

            return response;
        }
        private async Task<bool> IsAvailableFreeSigns(string json, SessionToken session)
        {
            if (!"esign-lite".Equals(session.ClientName, StringComparison.InvariantCultureIgnoreCase))
                return false;

            if (GetFileContentLength() > eSignLiteLimitation.FileSize ||
                GetRecipientsCount(json) > eSignLiteLimitation.Recipients)
                return false;

            var ipAddressHash = IpAddressDetector.IpAddress.GetShortMD5Hash();
            var freeNumberOfSigns = await _eSign.eSignPackageHistoryGetFreeNumberOfSigns(session.ClientId, eSignClient, ipAddressHash);

            return freeNumberOfSigns < eSignLiteLimitation.SignsPerDay;
        }


        [Route("{packageId:guid}"), HttpGet, HttpPost, HttpPut, HttpDelete]
        public async Task<HttpResponseMessage> Packages(Guid packageId, CancellationToken cancellationToken)
        {
            if (Request.Method == HttpMethod.Post || Request.Method == HttpMethod.Put)
            {
                string json = string.Empty;

                if (Request.Content.IsMimeMultipartContent("form-data"))
                {
                    var stream = await Request.Content.ReadAsStreamAsync();
                    var multipartAsync = await Request.Content.ReadAsMultipartAsync();

                    stream.Seek(0, System.IO.SeekOrigin.Begin);

                    foreach (var content in multipartAsync.Contents)
                    {
                        if ("\"package\"".Equals(content.Headers.ContentDisposition.Name, StringComparison.InvariantCultureIgnoreCase))
                        {
                            json = await content.ReadAsStringAsync();
                            break;
                        }
                    }
                }
                else
                    json = await Request.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(json))
                {
                    var jObject = JObject.Parse(json);
                    if (!object.Equals(jObject, null))
                    {
                        var property = jObject.Property("status");

                        if (!object.Equals(property, null) && "sent".Equals(property.Value.ToString(), StringComparison.InvariantCultureIgnoreCase))
                            return await Send(json, packageId, cancellationToken);
                    }
                }
            }

            return await HttpProxy(Request, Format("{0}", packageId), cancellationToken);
        }

        [Route("{packageId:guid}/evidence/summary"), HttpGet]
        public async Task<HttpResponseMessage> PackagesEvidenceSummary(Guid packageId, CancellationToken cancellationToken)
        {
            return await HttpProxy(Request, Format("{0}/evidence/summary", packageId), cancellationToken);
        }

        [Route("{packageId:guid}/decline"), HttpPost]
        public async Task<HttpResponseMessage> PackagesDecline(Guid packageId, CancellationToken cancellationToken)
        {
            return await HttpProxy(Request, Format("{0}/decline", packageId), cancellationToken);
        }

        [Route("{packageId:guid}/forward"), HttpPost]
        public async Task<HttpResponseMessage> PackagesForward(Guid packageId, CancellationToken cancellationToken)
        {
            return await HttpProxy(Request, Format("{0}/forward", packageId), cancellationToken);
        }

        [Route("{packageId:guid}/recipients/{recipientId:guid}/signingUrl"), HttpGet]
        public async Task<HttpResponseMessage> PackagesRecipientsSigningUrl(Guid packageId, Guid recipientId, CancellationToken cancellationToken)
        {
            return await HttpProxy(Request, Format("{0}/recipients/{1}/signingUrl", packageId, recipientId), cancellationToken);
        }

        [Route("{packageId:guid}/recipients/{recipientId:guid}/signature/appearance"), HttpPost]
        public async Task<HttpResponseMessage> PackagesRecipientsSignatureAppearance(Guid packageId, Guid recipientId, CancellationToken cancellationToken)
        {
            return await HttpProxy(Request, Format("{0}/recipients/{1}/signature/appearance", packageId, recipientId), cancellationToken);
        }

        [Route("{packageId:guid}/review"), HttpPost]
        public async Task<HttpResponseMessage> PackagesReview(Guid packageId, CancellationToken cancellationToken)
        {
            return await HttpProxy(Request, Format("{0}/review", packageId), cancellationToken);
        }

        [Route("{packageId:guid}/copy"), HttpPost]
        public async Task<HttpResponseMessage> PackagesCopy(Guid packageId, CancellationToken cancellationToken)
        {
            return await HttpProxy(Request, Format("{0}/copy", packageId), cancellationToken);
        }

        [Route("{packageId:guid}/recipients/{recipientId:guid}/notifications"), HttpPost]
        public async Task<HttpResponseMessage> PackagesRecipientsNotifications(Guid packageId, Guid recipientId, CancellationToken cancellationToken)
        {
            return await HttpProxy(Request, Format("{0}/recipients/{1}/notifications", packageId, recipientId), cancellationToken);
        }

        [Route("{packageId:guid}/documents/info"), HttpPost]
        public async Task<HttpResponseMessage> PackagesDocumentsInfo(Guid packageId, CancellationToken cancellationToken)
        {
            return await HttpProxy(Request, Format("{0}/documents/info", packageId), cancellationToken);
        }

        [Route("{packageId:guid}/documents/zip"), HttpGet]
        public async Task<HttpResponseMessage> PackagesDocumentsZip(Guid packageId, CancellationToken cancellationToken)
        {
            return await HttpProxy(Request, Format("{0}/documents/zip", packageId), cancellationToken);
        }

        [Route("{packageId:guid}/documents/{documentId:guid}/info"), HttpPost]
        public async Task<HttpResponseMessage> PackagesDocumentsInfo(Guid packageId, Guid documentId, CancellationToken cancellationToken)
        {
            return await HttpProxy(Request, Format("{0}/documents/{1}/info", packageId, documentId), cancellationToken);
        }

        [Route("{packageId:guid}/documents/{documentId:guid}/thumbnail"), HttpPost]
        public async Task<HttpResponseMessage> PackagesDocumentsThumbnail(Guid packageId, Guid documentId, CancellationToken cancellationToken)
        {
            return await HttpProxy(Request, Format("{0}/documents/{1}/thumbnail", packageId, documentId), cancellationToken);
        }

        [Route("{packageId:guid}/documents/{documentId:guid}/layout/{layoutId:guid}"), HttpPost]
        public async Task<HttpResponseMessage> PackagesDocumentsLayout(Guid packageId, Guid documentId, Guid layoutId, CancellationToken cancellationToken)
        {
            return await HttpProxy(Request, Format("{0}/documents/{1}/layout/{2}", packageId, documentId, layoutId), cancellationToken);
        }

        [Route("{packageId:guid}/documents/{documentId:guid}/download"), HttpGet]
        public async Task<HttpResponseMessage> PackagesDocumentsDownload(Guid packageId, Guid documentId, CancellationToken cancellationToken)
        {
            return await HttpProxy(Request, Format("{0}/documents/{1}/download", packageId, documentId), cancellationToken);
        }

        [Route("{packageId:guid}/documents/{documentId:guid}/download/original"), HttpGet]
        public async Task<HttpResponseMessage> PackagesDocumentsDownloadOriginal(Guid packageId, Guid documentId, CancellationToken cancellationToken)
        {
            return await HttpProxy(Request, Format("{0}/documents/{1}/download/original", packageId, documentId), cancellationToken);
        }

        [Route("{packageId:guid}/attachments/{attachmentId:guid}"), HttpPost, HttpDelete]
        public async Task<HttpResponseMessage> PackagesAttachments(Guid packageId, Guid attachmentId, CancellationToken cancellationToken)
        {
            return await HttpProxy(Request, Format("{0}/attachments/{1}", packageId, attachmentId), cancellationToken);
        }

        [Route("{packageId:guid}/attachments/{attachmentId:guid}/thumbnail"), HttpPost]
        public async Task<HttpResponseMessage> PackagesAttachmentsThumbnail(Guid packageId, Guid attachmentId, CancellationToken cancellationToken)
        {
            return await HttpProxy(Request, Format("{0}/attachments/{1}/thumbnail", packageId, attachmentId), cancellationToken);
        }

        [Route("{packageId:guid}/recipients/{recipientId:guid}/attachments/{attachmentId:guid}"), HttpPost]
        public async Task<HttpResponseMessage> PackagesRecipientAattachments(Guid packageId, Guid recipientId, Guid attachmentId, CancellationToken cancellationToken)
        {
            return await HttpProxy(Request, Format("{0}/recipients/{1}/attachments/{2}", packageId, recipientId, attachmentId), cancellationToken);
        }

        [Route("{packageId:guid}/recipients/{recipientId:guid}/attachments/{attachmentId:guid}/download"), HttpGet]
        public async Task<HttpResponseMessage> PackagesRecipientAattachmentsDownload(Guid packageId, Guid recipientId, Guid attachmentId, CancellationToken cancellationToken)
        {
            return await HttpProxy(Request, Format("{0}/recipients/{1}/attachments/{2}/download", packageId, recipientId, attachmentId), cancellationToken);
        }

        [Route("{packageId:guid}/recipients/{recipientId:guid}/attachments/zip"), HttpGet]
        public async Task<HttpResponseMessage> PackagesRecipientAattachmentsZip(Guid packageId, Guid recipientId, CancellationToken cancellationToken)
        {
            return await HttpProxy(Request, Format("{0}/recipients/{1}/attachments/zip", packageId, recipientId), cancellationToken);
        }
    }
}
