using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaaS.Data.Entities.Oauth
{
    public class SessionToken : AccountEntity<Guid>
    {
        public Guid? ParentId { get; set; }

        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public string ClientVersion { get; set; }

        [Column("externalClientId")]
        public ExternalClient? ExternalClient { get; set; }
        public string ExternalClientName { get; set; }
        public DateTime IssuedUtc { get; set; }
        public DateTime ExpiresUtc { get; set; }

        public string Scope { get; set; }

        [Required]
        public string ProtectedTicket { get; set; }
        public Guid? SystemId { get; set; }
        public Guid? AccountProductId { get; set; }
        public Guid? InstallationID { get; set; }
    }

    public static class SessionTokenHelper
    {
        public static Version GetClientVersion(this SessionToken sessionToken)
        {
            Version version;
            if (!Version.TryParse(sessionToken.ClientVersion, out version))
                return null;

            return version;
        }
    }
}