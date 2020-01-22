using Microsoft.AspNet.Identity;
using SaaS.Data.Entities.Admin;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaaS.Identity.Admin
{
    public class User : Entity<Guid>, IUser<Guid>
    {
        [Required, MaxLength(330)]
        public string Login { get; set; }

        [DataType(DataType.Password), MaxLength(50)]
        public string Password { get; set; }

        public bool IsActive { get; set; }

        [NotMapped]
        public string UserName
        {
            get { return string.Empty; }
            set { }
        }
    }
}
