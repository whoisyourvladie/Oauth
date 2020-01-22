using System;
using System.ComponentModel.DataAnnotations;

namespace SaaS.Api.Models.Oauth
{
    public class AuthViewModel
    {
        [Required, DataType(DataType.EmailAddress)]
        [RegularExpression(@"(?i)^[-a-z0-9!#$%&'*+\/=?^_`{|}~]+(\.[-a-z0-9!#$%&'*+\/=?^_`{|}~]+)*@([a-z0-9]([-a-z0-9]{0,61}[a-z0-9])?\.)*[a-z]{1,20}$")]
        public string Email { get; set; }
    }

    public class AuthIdViewModel
    {
        public Guid AccountId { get; set; }
    }

    public class AuthNameViewModel : AuthViewModel
    {
        [MaxLength(300), RegularExpression(@"^[^\^<>()\[\]\\;:@№%\|$%*?#&¸¼!+€£¢¾½÷×¤¶»¥¦µ©®°§«´¨™±°º¹²³ª¯¬`""'/]*$")]
        public string FirstName { get; set; }

        [MaxLength(300), RegularExpression(@"^[^\^<>()\[\]\\;:@№%\|$%*?#&¸¼!+€£¢¾½÷×¤¶»¥¦µ©®°§«´¨™±°º¹²³ª¯¬`""'/]*$")]
        public string LastName { get; set; }
    }

    public class AuthPasswordViewModel : AuthViewModel
    {
        public string Password { get; set; }
    }
}
