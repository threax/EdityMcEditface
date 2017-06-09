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
    public class SyncInfo : IHalLinkProvider
    {
        public int AheadBy { get; set; }

        public int BehindBy { get; set; }

        public IEnumerable<History> AheadHistory { get; set; }

        public IEnumerable<History> BehindHistory { get; set; }

        [JsonIgnore]
        public bool HasUncomittedChanges { get; set; }

        public IEnumerable<HalLinkAttribute> CreateHalLinks(ILinkProviderContext context)
        {
            if(AheadBy != 0)
            {
                yield return new HalActionLinkAttribute(SyncController.Rels.Push, typeof(SyncController));
            }

            if (BehindBy != 0)
            {
                yield return new HalActionLinkAttribute(SyncController.Rels.Pull, typeof(SyncController));
            }
        }
    }
}
