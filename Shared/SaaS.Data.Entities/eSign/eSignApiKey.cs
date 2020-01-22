using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaaS.Data.Entities.eSign
{
    public class eSignApiKey : AccountEntity<int>
    {
        [Column("eSignClientId")]
        public eSignClient eSignClient { get; set; }
        public string Key { get; set; }
        public string Email { get; set; }
        public bool IsNeedRefresh { get; set; }
        public DateTime CreateDate { get; set; }
    }
}