using SaaS.Api.Models.Validation;
using System.ComponentModel.DataAnnotations;

namespace SaaS.Api.Models.Api.Oauth
{
    public class ChangePasswordViewModel
    {
        [Required, DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required, DataType(DataType.Password), PasswordRegex]
        public string NewPassword { get; set; }
    }
}