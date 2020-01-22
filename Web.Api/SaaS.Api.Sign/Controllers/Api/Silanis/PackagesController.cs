using SaaS.Api.Core.Filters;
using System;
using System.Net.Http;
using System.Web.Http;

namespace SaaS.Api.Sign.Controllers.Api.Silanis
{
    [RoutePrefix("api/packages"), SaaSAuthorize]
    public class PackagesController : BaseApiController
    {
        protected override string ApiRoot
        {
            get { return "api/packages/"; }
        }

        [Route("{*url}"), HttpGet, HttpPost, HttpPut, HttpDelete]
        public HttpResponseMessage Index()
        {
            return HttpProxy(Request, Request.RequestUri.LocalPath);
        }

        [Route, HttpGet]
        public HttpResponseMessage Packages()
        {
            return HttpProxy(Request, Format());
        }

        [Route, HttpPost]
        public HttpResponseMessage PostPackages()
        {
            return HttpProxy(Request, Format());
        }

        [Route("{packageId:guid}"), HttpGet, HttpPost, HttpPut, HttpDelete]
        public HttpResponseMessage PackageId(Guid packageId)
        {
            return HttpProxy(Request, Format("{0}", packageId));
        }

        [Route("{packageId:guid}/clone"), HttpPost]
        public HttpResponseMessage PackageIdClone(Guid packageId)
        {
            return HttpProxy(Request, Format("{0}/clone", packageId));
        }

        [Route("{packageId:guid}/signingStatus"), HttpGet]
        public HttpResponseMessage PackageIdSigningStatus(Guid packageId)
        {
            return HttpProxy(Request, Format("{0}/signingStatus", packageId));
        }

        [Route("{packageId:guid}/evidence/summary"), HttpGet]
        public HttpResponseMessage PackageIdEvidenceSummary(Guid packageId)
        {
            return HttpProxy(Request, Format("{0}/evidence/summary", packageId));
        }

        #region Documents

        [Route("{packageId:guid}/documents"), HttpPost, HttpPut]
        public HttpResponseMessage PackageIdDocuments(Guid packageId)
        {
            return HttpProxy(Request, Format("{0}/documents", packageId));
        }

        [Route("{packageId:guid}/documents/zip"), HttpGet]
        public HttpResponseMessage PackageIdDocumentsZip(Guid packageId)
        {
            return HttpProxy(Request, Format("{0}/documents/zip", packageId));
        }

        [Route("{packageId:guid}/documents/{documentId}"), HttpGet, HttpPut, HttpDelete]
        public HttpResponseMessage PackageIdDocumentsDocumentId(Guid packageId, string documentId)
        {
            return HttpProxy(Request, Format("{0}/documents/{1}", packageId, documentId));
        }

        [Route("{packageId:guid}/documents/{documentId}/pdf"), HttpGet]
        public HttpResponseMessage PackageIdDocumentsDocumentIdPdf(Guid packageId, string documentId)
        {
            return HttpProxy(Request, Format("{0}/documents/{1}/pdf", packageId, documentId));
        }

        [Route("{packageId:guid}/documents/{documentId}/actions"), HttpPost]
        public HttpResponseMessage PackageIdDocumentsDocumentIdActions(Guid packageId, string documentId)
        {
            return HttpProxy(Request, Format("{0}/documents/{1}/actions", packageId, documentId));
        }

        [Route("{packageId:guid}/documents/{documentId}/layout"), HttpPost]
        public HttpResponseMessage PackageIdDocumentsDocumentIdLayout(Guid packageId, string documentId)
        {
            return HttpProxy(Request, Format("{0}/documents/{1}/layout", packageId, documentId));
        }

        [Route("{packageId:guid}/documents/{documentId}/approvals"), HttpPost, HttpPut]
        public HttpResponseMessage PackageIdDocumentsDocumentIdApprovals(Guid packageId, string documentId)
        {
            return HttpProxy(Request, Format("{0}/documents/{1}/approvals", packageId, documentId));
        }

        [Route("{packageId:guid}/documents/{documentId}/approvals/{approvalId}"), HttpGet, HttpPut, HttpDelete]
        public HttpResponseMessage PackageIdDocumentsDocumentIdApprovalsApprovalId(Guid packageId, string documentId, string approvalId)
        {
            return HttpProxy(Request, Format("{0}/documents/{1}/approvals/{2}", packageId, documentId, approvalId));
        }

        [Route("{packageId:guid}/documents/{documentId}/approvals/{approvalId}/fields"), HttpPost]
        public HttpResponseMessage PackageIdDocumentsDocumentIdApprovalsApprovalIdFields(Guid packageId, string documentId, string approvalId)
        {
            return HttpProxy(Request, Format("{0}/documents/{1}/approvals/{2}/fields", packageId, documentId, approvalId));
        }

        [Route("{packageId:guid}/documents/{documentId}/approvals/{approvalId}/fields/{fieldId}"), HttpGet, HttpPost, HttpPut, HttpDelete]
        public HttpResponseMessage PackageIdDocumentsDocumentIdApprovalsApprovalIdFields(Guid packageId, string documentId, string approvalId, string fieldId)
        {
            return HttpProxy(Request, Format("{0}/documents/{1}/approvals/{2}/fields/{3}", packageId, documentId, approvalId, fieldId));
        }

        [Route("{packageId:guid}/documents/{documentId}/approvals/{approvalId}/sign"), HttpPost]
        public HttpResponseMessage PackageIdDocumentsDocumentIdApprovalsApprovalIdSign(Guid packageId, string documentId, string approvalId)
        {
            return HttpProxy(Request, Format("{0}/documents/{1}/approvals/{2}/sign", packageId, documentId, approvalId));
        }

        #endregion

        #region Roles

        [Route("{packageId:guid}/roles"), HttpGet, HttpPost, HttpPut]
        public HttpResponseMessage PackageIdRoles(Guid packageId)
        {
            return HttpProxy(Request, Format("{0}/roles", packageId));
        }

        [Route("{packageId:guid}/roles/{roleId}"), HttpGet, HttpPut, HttpDelete]
        public HttpResponseMessage PackageIdRolesRoleId(Guid packageId, string roleId)
        {
            return HttpProxy(Request, Format("{0}/roles/{1}", packageId, roleId));
        }

        [Route("{packageId:guid}/roles/{roleId}/notifications"), HttpPost]
        public HttpResponseMessage PackageIdRolesRoleIdNotifications(Guid packageId, string roleId)
        {
            return HttpProxy(Request, Format("{0}/roles/{1}/notifications", packageId, roleId));
        }

        [Route("{packageId:guid}/roles/{roleId}/sms_notification"), HttpPost]
        public HttpResponseMessage PackageIdRolesRoleIdSmsNotification(Guid packageId, string roleId)
        {
            return HttpProxy(Request, Format("{0}/roles/{1}/sms_notification", packageId, roleId));
        }

        [Route("{packageId:guid}/roles/{roleId}/unlock"), HttpPost]
        public HttpResponseMessage PackageIdRolesRoleIdUnlock(Guid packageId, string roleId)
        {
            return HttpProxy(Request, Format("{0}/roles/{1}/unlock", packageId, roleId));
        }

        [Route("{packageId:guid}/roles/{roleId}/signingUrl"), HttpPost]
        public HttpResponseMessage PackageIdRolesRoleIdSigningUrl(Guid packageId, string roleId)
        {
            return HttpProxy(Request, Format("{0}/roles/{1}/signingUrl", packageId, roleId));
        }

        #endregion
    }
}