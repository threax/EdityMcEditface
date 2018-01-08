using Halcyon.HAL.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using Threax.AspNetCore.Halcyon.Ext;

namespace EdityMcEditface.Mvc.Models.Branch
{
    [HalModel]
    public class BranchCollection : CollectionView<BranchView>
    {
        public BranchCollection(IEnumerable<BranchView> items) : base(items)
        {
        }
    }
}
