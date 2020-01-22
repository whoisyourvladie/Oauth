using System;

namespace SaaS.Data.Entities.eSign
{
    public class eSignPackageHistory : AccountEntity<int>
    {
        public Guid? AccountProductId { get; set; }
        public Guid? AccountMicrotransactionId { get; set; }
        public int oAuthClientId { get; set; }
        public eSignClient eSignClientId { get; set; }
        public Guid PackageId { get; set; }
        public bool IsSuccess { get; set; }
        public int HttpStatusCode { get; set; }
        public DateTime CreateDate { get; set; }
        public string IpAddressHash { get; set; }
    }
}