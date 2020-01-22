using System;

namespace SaaS.Data.Entities.View
{
    public class ViewAccountSystem
    {
        public Guid SystemId { get; set; }
        public Guid AccountId { get; set; }
        public Guid AccountProductId { get; set; }
        public string PcName { get; set; }
    }
}