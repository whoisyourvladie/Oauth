using System.ComponentModel.DataAnnotations.Schema;

namespace SaaS.Data.Entities.Accounts
{
    public class AccountPreference : AccountEntity<int>
    {
        [Column("json")]
        public string Json { get; set; }
    }
}