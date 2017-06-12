using Halcyon.HAL.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using Threax.AspNetCore.Halcyon.Ext;

namespace EdityMcEditface.Mvc.Models
{
    [HalModel]
    public class HistoryQuery : PagedCollectionQuery
    {
        public String FilePath { get; set; }
    }
}
