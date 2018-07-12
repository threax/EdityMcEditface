using EdityMcEditface.HtmlRenderer;
using EdityMcEditface.Mvc.Controllers;
using Halcyon.HAL.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using Threax.AspNetCore.Halcyon.Ext;

namespace EdityMcEditface.Mvc.Models.Page
{
    [HalModel]
    [HalSelfActionLink(PageController.Rels.List, typeof(PageController))]
    [DeclareHalLink(PagedCollectionView<Object>.Rels.Next, PageController.Rels.List, typeof(PageController), ResponseOnly = true)]
    [DeclareHalLink(PagedCollectionView<Object>.Rels.Previous, PageController.Rels.List, typeof(PageController), ResponseOnly = true)]
    [DeclareHalLink(PagedCollectionView<Object>.Rels.First, PageController.Rels.List, typeof(PageController), ResponseOnly = true)]
    [DeclareHalLink(PagedCollectionView<Object>.Rels.Last, PageController.Rels.List, typeof(PageController), ResponseOnly = true)]
    public class PageInfoCollection : PagedCollectionView<PageInfo>
    {
        private PageQuery query;

        public PageInfoCollection(PageQuery query, int total, IEnumerable<PageInfo> items) : base(query, total, items)
        {
            this.query = query;
        }

        protected override void AddCustomQuery(string rel, RequestDataBuilder queryString)
        {
            if (query.File != null)
            {
                queryString.AppendItem("file", query.File.TrimStartingPathChars());
            }

            base.AddCustomQuery(rel, queryString);
        }
    }
}
