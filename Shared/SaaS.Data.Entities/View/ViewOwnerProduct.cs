using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaaS.Data.Entities.View
{
    public class ViewOwnerProduct : ViewProduct
    {
        public string OwnerEmail { get; set; }
        public int AllowedCount { get; set; }
        public int UsedCount { get; set; }

        [NotMapped]
        public List<ViewOwnerAccount> Accounts { get; set; }
    }
}