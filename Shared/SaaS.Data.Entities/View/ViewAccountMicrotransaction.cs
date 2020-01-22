using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaaS.Data.Entities.View
{
    public class ViewAccountMicrotransaction : ViewMicrotransaction
    {
        [NotMapped]
        public List<ViewAccountMicrotransactionModule> Modules { get; set; }
    }
}