using Newtonsoft.Json;
using SaaS.Api.Models.Validation;
using System;
using System.ComponentModel.DataAnnotations;

namespace SaaS.Api.Models.Api.Oauth
{
    public class RegisterViewModel : AuthNameViewModel
    {
        private string _build = null;

        [DataType(DataType.Password), PasswordRegex(IsAllowNull = true)]
        public string Password { get; set; }

        [MaxLength(128)]
        public string Source { get; set; }

        [MaxLength(10)]
        public string Build
        {
            get { return _build; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;

                _build = value.Trim();
            }
        }

        public bool IsBusiness()
        {
            return string.Equals(Build, "b2b", StringComparison.InvariantCultureIgnoreCase);
        }

        [MaxLength(2), JsonProperty("Country")]
        public string CountryISO2 { get; set; }

        public ParamsViewModel Params { get; set; }
        

        public string Phone { get; set; }
        public string PhoneESign { get; set; }
        public string Company { get; set; }
        public string Occupation { get; set; }
        public string LanguageISO2 { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string State { get; set; }

        public string Industry { get; set; }
        public string JobRole { get; set; }
        public string Licenses { get; set; }
        public string Product { get; set; }

        public Guid? VisitorId { get; set; }
    }
}