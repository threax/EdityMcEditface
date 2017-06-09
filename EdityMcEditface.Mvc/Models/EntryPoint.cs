using EdityMcEditface.Mvc.Controllers;
using Halcyon.HAL.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using Threax.AspNetCore.Halcyon.Ext;

namespace EdityMcEditface.Mvc.Models
{
    [HalModel]
    [HalEntryPoint]
    [HalSelfActionLink(EntryPointController.Rels.Get, typeof(EntryPointController))]

    [HalActionLink(PhaseController.Rels.List, typeof(PhaseController))]

    [HalActionLink(DraftController.Rels.Begin, typeof(DraftController))]
    [HalActionLink(DraftController.Rels.List, typeof(DraftController))]
    [HalActionLink(DraftController.Rels.SubmitAllDrafts, typeof(DraftController))]

    [HalActionLink(CommitController.Rels.Commit, typeof(CommitController))]
    [HalActionLink(CommitController.Rels.GetUncommittedChanges, typeof(CommitController))]

    [HalActionLink(SyncController.Rels.BeginSync, typeof(SyncController))]
    public class EntryPoint
    {
    }
}
