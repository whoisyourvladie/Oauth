using System;

namespace SaaS.PdfEscape.Api.Client
{
    public class PdfEscapeApiToken
    {
        public PdfEscapeApiUser User { get; set; }
        public PdfEscapeApiMembership Membership { get; set; }
    }

    public class PdfEscapeApiUser
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class PdfEscapeApiMembership
    {
        public DateTime Expiration { get; set; }
        public uint Status { get; set; }
        public uint Type { get; set; }
    }
}
