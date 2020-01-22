using Newtonsoft.Json;
using SaaS.Api.Models.Validation;
using System.ComponentModel.DataAnnotations;

namespace SaaS.Api.Models.Api.Oauth
{
    public class RegisterAnonymousViewModel
    {
        [Required, DataType(DataType.Password), PasswordRegex]
        public string Password { get; set; }

        public string client_id { get; set; }

        //[MaxLength(128)]
        //public string Source { get; set; }
    }
}