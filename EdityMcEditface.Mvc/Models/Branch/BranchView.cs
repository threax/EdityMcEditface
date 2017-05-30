using EdityMcEditface.Mvc.Controllers;
using Halcyon.HAL.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using Threax.AspNetCore.Halcyon.Ext;

namespace EdityMcEditface.Mvc.Models.Branch
{
    [HalModel]
    [HalActionLink(BranchController.Rels.SetBranch, typeof(BranchController))]
    public class BranchView
    {
        public String Name { get; set; }

        public bool Current { get; set; }
    }
}
