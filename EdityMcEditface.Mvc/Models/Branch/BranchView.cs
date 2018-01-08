using Halcyon.HAL.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace EdityMcEditface.Mvc.Models.Branch
{
    [HalModel]
    public class BranchView
    {
        public String CanonicalName { get; set; }

        public String FriendlyName { get; set; }
    }
}
