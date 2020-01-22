using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaaS.Data.Entities.View
{
    public class ViewAccountDetails : Entity<Guid>
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string PhoneESign { get; set; }
        public string Company { get; set; }
        public string Occupation { get; set; }
        public string CountryISO2 { get; set; }
        public string LanguageISO2 { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string State { get; set; }
        public string Build { get; set; }
        public string Cmp { get; set; }
        public int? Uid { get; set; }
        public string GeoIp { get; set; }
        public string Partner { get; set; }

        public bool IsAnonymous { get; set; }
        public bool IsActivated { get; set; }
        public bool IsBusiness { get; set; }
#if LuluSoft
        public bool? IsPreview { get; set; }
        public int? TrialDays { get; set; }
#endif
        public bool? Optin { get; set; }
        public bool? IsTrial { get; set; }
        public Guid? InstallationID { get; set; }

        [NotMapped]
        public ulong Status
        {
            get
            {
                ulong status = 0;

                status |= (ulong)(IsActivated ? AccountStatus.IsActivated : 0);
                status |= (ulong)(IsAnonymous ? AccountStatus.IsAnonymous : 0);
                status |= (ulong)(IsBusiness ? AccountStatus.IsBusiness : 0);
#if LuluSoft
                status |= (ulong)(IsPreview == true ? AccountStatus.IsPreview : 0);
#endif

                return status;
            }
        }

        [NotMapped]
        public string Source { get; set; }

        [NotMapped]
        public string WebForm { get; set; }
    }
}
