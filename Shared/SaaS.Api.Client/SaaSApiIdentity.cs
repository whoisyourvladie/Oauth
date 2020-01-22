using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace SaaS.Api.Client
{
    public class SaaSApiIdentity : ClaimsIdentity
    {
        public const string RolesClaimType = "http://www.assilabdulrahim.com/CuttingEdge.Security.Role";
        public const string GroupClaimType = "http://www.assilabdulrahim.com/CuttingEdge.Security.Group";
        public const string IPClaimType = "http://www.assilabdulrahim.com/CuttingEdge.Security.IP";
        public const string IdClaimType = "http://www.assilabdulrahim.com/CuttingEdge.Security.Id";


        public SaaSApiIdentity(IEnumerable<Claim> claims, string authenticationType)
            : base(claims, authenticationType: authenticationType)
        {

            AddClaims(from @group in claims where @group.Type == GroupClaimType select @group);
            AddClaims(from role in claims where role.Type == RoleClaimType select role);
            AddClaims(from id in claims where id.Type == IdClaimType select id);
            AddClaims(from ip in claims where ip.Type == IPClaimType select ip);


        }

        public SaaSApiIdentity(IEnumerable<string> roles, IEnumerable<string> groups, string IP, int id)
        {
            AddClaims(from @group in groups select new Claim(GroupClaimType, @group));
            AddClaims(from role in roles select new Claim(RolesClaimType, role));
            AddClaim(new Claim(IdClaimType, id.ToString()));
            AddClaim(new Claim(IPClaimType, IP.ToString()));
        }

        public IEnumerable<string> Roles
        {
            get
            {
                return from claim in FindAll(RolesClaimType) select claim.Value;
            }
        }

        public IEnumerable<string> Groups { get { return from claim in FindAll(GroupClaimType) select claim.Value; } }

        public int Id { get { return Convert.ToInt32(FindFirst(IdClaimType).Value); } }
        public string IP { get { return FindFirst(IPClaimType).Value; } }
    }
}
