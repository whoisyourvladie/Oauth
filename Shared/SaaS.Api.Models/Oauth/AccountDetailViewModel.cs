using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace SaaS.Api.Models.Oauth
{
    public class AccountDetailsViewModel : AuthNameViewModel
    {
        public Guid Id { get; set; }

        [MaxLength(20)]
        public string Phone { get; set; }
        [MaxLength(20)]

        public string PhoneESign { get; set; }

        [MaxLength(128)]
        public string Company { get; set; }

        [MaxLength(128)]
        public string Occupation { get; set; }

        [MaxLength(2), JsonProperty("Country")]
        public string CountryISO2 { get; set; }

        [MaxLength(2), JsonProperty("Language")]
        public string LanguageISO2 { get; set; }

        [MaxLength(128)]
        public string Address1 { get; set; }

        [MaxLength(128)]
        public string Address2 { get; set; }

        [MaxLength(128)]
        public string City { get; set; }

        [MaxLength(32)]
        public string PostalCode { get; set; }

        [MaxLength(128)]
        public string State { get; set; }

        public ulong Status { get; set; }
        public bool? Optin { get; set; }
    }
}
