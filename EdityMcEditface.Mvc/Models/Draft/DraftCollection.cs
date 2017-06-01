using EdityMcEditface.Mvc.Controllers;
using Halcyon.HAL.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using Threax.AspNetCore.Halcyon.Ext;

namespace EdityMcEditface.Mvc.Models
{
    [HalModel]
    [HalSelfActionLink(DraftController.Rels.List, typeof(DraftController))]
    public class DraftCollection : PagedCollectionView<Draft>
    {
        private DraftQuery query;

        public DraftCollection(DraftQuery query, int total, IEnumerable<Draft> items) : base(query, total, items)
        {
            this.query = query;
        }

        protected override void AddCustomQuery(string rel, QueryStringBuilder queryString)
        {
            if(query.File != null)
            {
                queryString.AppendItem("file", query.File);
            }

            base.AddCustomQuery(rel, queryString);
        }
    }
}
