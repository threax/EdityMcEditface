using EdityMcEditface.Mvc.Controllers;
using Halcyon.HAL.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using Threax.AspNetCore.Halcyon.Ext;

namespace EdityMcEditface.Mvc.Models.Branch
{
    [HalModel]
    [HalSelfActionLink(typeof(BranchController), nameof(BranchController.List))]
    [HalActionLink(typeof(BranchController), nameof(BranchController.Add))]
    [HalActionLink(typeof(BranchController), nameof(BranchController.Current))]
    public class BranchCollection : CollectionView<BranchView>
    {
        public BranchCollection(IEnumerable<BranchView> items) : base(items)
        {
        }
    }
}
