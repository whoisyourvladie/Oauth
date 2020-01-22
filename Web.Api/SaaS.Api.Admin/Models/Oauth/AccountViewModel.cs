using SaaS.Data.Entities.Accounts;
using System;

namespace SaaS.Api.Admin.Models.Oauth
{
    public class AccountViewModel
    {
        public AccountViewModel() { }

        public AccountViewModel(Account user)
        {
            Id = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;
            Status = user.GetStatus();
        }

        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public ulong Status { get; set; }
    }
}