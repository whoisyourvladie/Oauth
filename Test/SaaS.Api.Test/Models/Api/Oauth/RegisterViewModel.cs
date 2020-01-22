
namespace SaaS.Api.Test.Models.Api.Oauth
{
    public class RegisterViewModel : AuthViewModel
    {
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}