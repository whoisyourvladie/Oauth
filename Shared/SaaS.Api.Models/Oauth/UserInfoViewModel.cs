using System.ComponentModel.DataAnnotations;

namespace SaaS.Api.Models.Oauth
{
    public class UserInfoViewModel
    {
        [MaxLength(300), RegularExpression(@"^[^\^<>()\[\]\\;:@№%\|$%*?#&¸¼!+€£¢¾½÷×¤¶»¥¦µ©®°§«´¨™±°º¹²³ª¯¬`""'/]*$")]
        public string FirstName { get; set; }

        [MaxLength(300), RegularExpression(@"^[^\^<>()\[\]\\;:@№%\|$%*?#&¸¼!+€£¢¾½÷×¤¶»¥¦µ©®°§«´¨™±°º¹²³ª¯¬`""'/]*$")]
        public string LastName { get; set; }
    }
}
