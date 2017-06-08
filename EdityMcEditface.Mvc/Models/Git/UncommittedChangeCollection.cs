using EdityMcEditface.Mvc.Controllers;
using Halcyon.HAL.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using Threax.AspNetCore.Halcyon.Ext;

namespace EdityMcEditface.Mvc.Models.Git
{
    [HalModel]
    [HalSelfActionLink(CommitController.Rels.GetUncommittedChanges, typeof(CommitController))]
    public class UncommittedChangeCollection : CollectionView<UncommittedChange>
    {
        public UncommittedChangeCollection(IEnumerable<UncommittedChange> items) : base(items)
        {
        }
    }
}
