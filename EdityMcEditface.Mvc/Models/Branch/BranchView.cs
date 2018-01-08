using EdityMcEditface.Mvc.Controllers;
using Halcyon.HAL.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using Threax.AspNetCore.Halcyon.Ext;

namespace EdityMcEditface.Mvc.Models.Branch
{
    [HalModel]
    [HalActionLink(typeof(BranchController), nameof(BranchController.Checkout))]
    public class BranchView
    {
        public String CanonicalName { get; set; }

        public String FriendlyName { get; set; }
    }
}
