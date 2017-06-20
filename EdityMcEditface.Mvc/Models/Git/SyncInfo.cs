using EdityMcEditface.Mvc.Controllers;
using Halcyon.HAL.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Threax.AspNetCore.Halcyon.Ext;

namespace EdityMcEditface.Mvc.Models.Git
{
    [HalModel]
    [HalSelfActionLink(SyncController.Rels.BeginSync, typeof(SyncController))]
    [DeclareHalLink(SyncController.Rels.Pull, typeof(SyncController))]
    [DeclareHalLink(SyncController.Rels.Push, typeof(SyncController))]
    [DeclareHalLink(CommitController.Rels.Commit, typeof(CommitController))]
    public class SyncInfo : IHalLinkProvider
    {
        public int AheadBy { get; set; }

        public int BehindBy { get; set; }

        public IEnumerable<History> AheadHistory { get; set; }

        public IEnumerable<History> BehindHistory { get; set; }

        [JsonIgnore]
        public bool HasUncomittedChanges { get; set; }

        [JsonIgnore]
        public bool NeedsPull
        {
            get
            {
                return BehindBy != 0;
            }
        }

        [JsonIgnore]
        public bool NeedsPush
        {
            get
            {
                return AheadBy != 0;
            }
        }

        public IEnumerable<HalLinkAttribute> CreateHalLinks(ILinkProviderContext context)
        {
            if (HasUncomittedChanges)
            {
                yield return new HalActionLinkAttribute(CommitController.Rels.Commit, typeof(CommitController));
            }
            else
            {
                if (NeedsPush)
                {
                    yield return new HalActionLinkAttribute(SyncController.Rels.Push, typeof(SyncController));
                }

                if (NeedsPull)
                {
                    yield return new HalActionLinkAttribute(SyncController.Rels.Pull, typeof(SyncController));
                }
            }
        }
    }
}
