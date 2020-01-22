using System;

namespace SaaS.Data.Entities.Admin.View
{
    public class ViewLog
    {
        public string Login { get; set; }
        public string Role { get; set; }

        public string Log { get; set; }
        public Guid? AccountId { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
