using System;

namespace SaaS.Data.Entities.View.Oauth
{
    public class ViewSessionToken
    {
        public Guid RefreshToken { get; set; }
        public Guid AccountId { get; set; }
        public Guid AccountProductId { get; set; }
        public string PcName { get; set; }
        public string Client { get; set; }
    }
}