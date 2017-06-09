using EdityMcEditface.Mvc.Controllers;
using Halcyon.HAL.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Threax.AspNetCore.Halcyon.Ext;

namespace EdityMcEditface.Mvc.Models
{
    [HalModel]
    [HalEntryPoint]
    [HalSelfActionLink(DraftController.Rels.Begin, typeof(DraftController))]
    [DeclareHalLink(CommitController.Rels.Commit, typeof(CommitController))]
    [DeclareHalLink(DraftController.Rels.List, typeof(DraftController))]
    public class DraftEntryPoint : IHalLinkProvider
    {
        [JsonIgnore]
        public bool HasUncommittedChanges { get; set; }

        public IEnumerable<HalLinkAttribute> CreateHalLinks(ILinkProviderContext context)
        {
            if (HasUncommittedChanges)
            {
                yield return new HalActionLinkAttribute(CommitController.Rels.Commit, typeof(CommitController));
            }
            else
            {
                yield return new HalActionLinkAttribute(DraftController.Rels.List, typeof(DraftController));
            }
        }
    }
}
