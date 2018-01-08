using EdityMcEditface.Mvc.Controllers;
using Halcyon.HAL.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using Threax.AspNetCore.Halcyon.Ext;

namespace EdityMcEditface.Mvc.Models.Phase
{
    [HalModel]
    [HalSelfActionLink(PhaseController.Rels.List, typeof(PhaseController))]
    public class PhaseCollection : CollectionView<Phase>
    {
        public PhaseCollection(IEnumerable<Phase> items) : base(items)
        {
        }
    }
}
