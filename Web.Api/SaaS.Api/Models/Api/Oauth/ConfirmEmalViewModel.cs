using System;

namespace SaaS.Api.Models.Api.Oauth
{
    public class ConfirmEmalViewModel
    {
        public string Token { get; set; }
        public Guid UserId { get; set; }
        public Guid? VisitorId { get; set; }
        public string Build { get; set; }

        public bool IsBusiness()
        {
            return "b2b".Equals(Build, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}