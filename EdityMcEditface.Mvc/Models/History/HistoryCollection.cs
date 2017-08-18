using EdityMcEditface.Mvc.Controllers;
using Halcyon.HAL.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using Threax.AspNetCore.Halcyon.Ext;

namespace EdityMcEditface.Mvc.Models
{
    [HalModel]
    [HalSelfActionLink(HistoryController.Rels.ListHistory, typeof(HistoryController))]
    [HalActionLink(CrudRels.List, HistoryController.Rels.ListHistory, typeof(HistoryController), DocsOnly = true)]
    [DeclareHalLink(PagedCollectionView<Object>.Rels.Next, HistoryController.Rels.ListHistory, typeof(HistoryController), ResponseOnly = true)]
    [DeclareHalLink(PagedCollectionView<Object>.Rels.Previous, HistoryController.Rels.ListHistory, typeof(HistoryController), ResponseOnly = true)]
    [DeclareHalLink(PagedCollectionView<Object>.Rels.First, HistoryController.Rels.ListHistory, typeof(HistoryController), ResponseOnly = true)]
    [DeclareHalLink(PagedCollectionView<Object>.Rels.Last, HistoryController.Rels.ListHistory, typeof(HistoryController), ResponseOnly = true)]
    public class HistoryCollection : PagedCollectionView<History>
    {
        public HistoryCollection(HistoryQuery query, int total, IEnumerable<History> items) : base(query, total, items)
        {
            this.FilePath = query.FilePath;
        }

        public String FilePath { get; set; }
    }
}
