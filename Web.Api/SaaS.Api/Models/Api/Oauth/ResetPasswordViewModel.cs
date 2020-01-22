using SaaS.Api.Models.Validation;
using System;
using System.ComponentModel.DataAnnotations;

namespace SaaS.Api.Models.Api.Oauth
{
    public class ResetPasswordViewModel
    {
        [Required]
        public Guid UserId { get; set; }
        public Guid? VisitorId { get; set; }

        public string Token { get; set; }

        [Required, DataType(DataType.Password), PasswordRegex]
        public string NewPassword { get; set; }

        [MaxLength(300), RegularExpression(@"^[^\^<>()\[\]\\;:@№%\|$%*?#&¸¼!+€£¢¾½÷×¤¶»¥¦µ©®°§«´¨™±°º¹²³ª¯¬`""'/]*$")]
        public string FirstName { get; set; }

        [MaxLength(300), RegularExpression(@"^[^\^<>()\[\]\\;:@№%\|$%*?#&¸¼!+€£¢¾½÷×¤¶»¥¦µ©®°§«´¨™±°º¹²³ª¯¬`""'/]*$")]
        public string LastName { get; set; }
    }
}