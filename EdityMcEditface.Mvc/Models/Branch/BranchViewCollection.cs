using EdityMcEditface.Mvc.Controllers;
using Halcyon.HAL.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using Threax.AspNetCore.Halcyon.Ext;

namespace EdityMcEditface.Mvc.Models.Branch
{
    [HalModel]
    [HalSelfActionLink(BranchController.Rels.List, typeof(BranchController))]
    public class BranchViewCollection : CollectionView<BranchView>
    {
        public BranchViewCollection(IEnumerable<BranchView> items) : base(items)
        {
        }
    }
}
