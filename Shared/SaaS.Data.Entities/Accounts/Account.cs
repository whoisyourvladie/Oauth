using Microsoft.AspNet.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaaS.Data.Entities.Accounts
{
    public class Account : Entity<Guid>, IUser<Guid>
    {
        public Account()
        {
            this.RefreshModifyDate();
        }

        public Account(string email)
            : this()
        {
            Email = email;
        }
        public Account(string email, string firstName, string lastName) :
            this(email)
        {
            FirstName = firstName;
            LastName = lastName;
        }

        [Required, MaxLength(330)]
        public string Email { get; set; }

        [DataType(DataType.Password), MaxLength(50)]
        public string Password { get; set; }

        [MaxLength(300)]
        public string FirstName { get; set; }

        [MaxLength(300)]
        public string LastName { get; set; }

        public bool? Optin { get; set; }
        public bool NeverLogged { get; set; }
        public bool IsAnonymous { get; set; }
        public bool IsActivated { get; set; }
        public bool IsBusiness { get; set; }
#if LuluSoft
        public bool? IsPreview { get; set; }
#endif

        [DataType(DataType.DateTime)]
        public DateTime ModifyDate { get; set; }

        [NotMapped]
        public string UserName
        {
            get { return string.Format("{0} {1}", FirstName, LastName); }
            set { }
        }
    }

    public static class AccountHelper
    {
        public static bool IsEmptyPassword(this Account account)
        {
            return string.IsNullOrEmpty(account.Password);
        }
        public static ulong GetStatus(this Account account)
        {
            ulong status = 0;

            status |= (ulong)(account.IsActivated ? AccountStatus.IsActivated : 0);
            status |= (ulong)(account.IsAnonymous ? AccountStatus.IsAnonymous : 0);
            status |= (ulong)(account.IsBusiness ? AccountStatus.IsBusiness : 0);
#if LuluSoft
            status |= (ulong)(account.IsPreview == true ? AccountStatus.IsPreview : 0);
#endif

            return status;
        }
        public static void RefreshModifyDate(this Account account)
        {
            account.ModifyDate = DateTime.UtcNow;
        }
    }
}
