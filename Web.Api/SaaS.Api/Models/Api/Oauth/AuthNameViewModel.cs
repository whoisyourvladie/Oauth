using System.ComponentModel.DataAnnotations;

namespace SaaS.Api.Models.Api.Oauth
{
    public class AuthNameViewModel : AuthViewModel
    {
        [MaxLength(300), RegularExpression(@"^[^\^<>()\[\]\\;:@№%\|$%*?#&¸¼!+€£¢¾½÷×¤¶»¥¦µ©®°§«´¨™±°º¹²³ª¯¬`""'/]*$")]
        public string FirstName { get; set; }

        [MaxLength(300), RegularExpression(@"^[^\^<>()\[\]\\;:@№%\|$%*?#&¸¼!+€£¢¾½÷×¤¶»¥¦µ©®°§«´¨™±°º¹²³ª¯¬`""'/]*$")]
        public string LastName { get; set; }
    }
}