using SaaS.Data.Entities.View.Oauth;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaaS.Data.Entities.View
{
    public class ViewOwnerAccount
    {
        public Guid AccountId { get; set; }
        public string Email { get; set; }

        [NotMapped]
        public List<ViewAccountSystem> AccountSystems { get; set; }

        [NotMapped]
        public List<ViewSessionToken> SessionTokens { get; set; }
    }
}