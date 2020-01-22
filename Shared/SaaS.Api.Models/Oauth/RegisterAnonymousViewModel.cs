using SaaS.Api.Models.Validation;
using System.ComponentModel.DataAnnotations;

namespace SaaS.Api.Models.Oauth
{
    public class RegisterAnonymousViewModel
    {
        [Required, DataType(DataType.Password), PasswordRegex]
        public string Password { get; set; }

        public string client_id { get; set; }

        public string client_version { get; set; }

        //[MaxLength(128)]
        //public string Source { get; set; }
    }
}