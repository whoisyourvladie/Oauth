namespace SaaS.Api.Models.Oauth
{
    public class EmailChangePendingViewModel : AuthViewModel
    {
        public string[] PendingEmails { get; set; }
    }
}
