using SaaS.Api.Models.Validation;
using System;
using System.ComponentModel.DataAnnotations;

namespace SaaS.Api.Models.Oauth
{
    public class ChangePasswordViewModel
    {
        [Required, DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required, DataType(DataType.Password), PasswordRegex]
        public string NewPassword { get; set; }
    }

    public class SetPasswordViewModel
    {
        [Required]
        public Guid Id { get; set; }

        [DataType(DataType.Password), PasswordRegex(IsAllowNull = true)]
        public string NewPassword { get; set; }

        public bool IsConnect { get; set; }
    }
}