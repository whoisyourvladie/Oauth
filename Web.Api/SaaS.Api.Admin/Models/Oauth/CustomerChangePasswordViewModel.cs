using System.ComponentModel.DataAnnotations;

namespace SaaS.Api.Admin.Models.Oauth
{
    public class CustomerChangePasswordViewModel
    {
        [Required, DataType(DataType.Password)]
        public string NewPassword { get; set; }
    }
}