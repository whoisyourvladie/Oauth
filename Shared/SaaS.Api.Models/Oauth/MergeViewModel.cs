using System;

namespace SaaS.Api.Models.Oauth
{
    public class MergeViewModel
    {
        public Guid AccountIdFrom { get; set; }
        public Guid AccountIdPrimaryEmail { get; set; }
    }
}