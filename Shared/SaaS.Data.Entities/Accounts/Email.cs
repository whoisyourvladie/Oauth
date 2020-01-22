using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaaS.Data.Entities.Accounts
{
    public class Email : AccountEntity<int>
    {
        [Column("status")]
        public Status StatusId { get; set; }

        [Column("emailCustomParam", TypeName = "xml")]
        public string EmailCustomParam { get; set; }

        [Column("modelCustomParam", TypeName = "xml")]
        public string ModelCustomParam { get; set; }

        [Column("languageID")]
        public string LanguageId { get; set; }

        [Column("createDate")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreateDate { get; set; }
    }
}