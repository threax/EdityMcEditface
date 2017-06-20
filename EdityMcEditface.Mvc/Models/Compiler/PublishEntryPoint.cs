using EdityMcEditface.Mvc.Controllers;
using Halcyon.HAL.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Threax.AspNetCore.Halcyon.Ext;

namespace EdityMcEditface.Mvc.Models.Compiler
{
    [HalModel]
    [HalSelfActionLink(PublishController.Rels.BeginPublish, typeof(PublishController))]
    [DeclareHalLink(PublishController.Rels.Compile, typeof(PublishController))]
    [DeclareHalLink(SyncController.Rels.BeginSync, typeof(SyncController))]
    [DeclareHalLink(CommitController.Rels.Commit, typeof(CommitController))]
    public class PublishEntryPoint : IHalLinkProvider
    {
        public int BehindBy { get; internal set; }

        public IEnumerable<History> BehindHistory { get; internal set; }

        [JsonIgnore]
        public bool HasUncommittedChanges { get; set; }

        [JsonIgnore]
        public bool HasUnsyncedChanges { get; set; }

        public IEnumerable<HalLinkAttribute> CreateHalLinks(ILinkProviderContext context)
        {
            if (HasUncommittedChanges)
            {
                yield return new HalActionLinkAttribute(CommitController.Rels.Commit, typeof(CommitController));
            }
            else if (HasUnsyncedChanges)
            {
                yield return new HalActionLinkAttribute(SyncController.Rels.BeginSync, typeof(SyncController));
            }
            else
            {
                yield return new HalActionLinkAttribute(PublishController.Rels.Compile, typeof(PublishController));
            }
        }
    }
}
