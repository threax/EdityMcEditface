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

    [HalActionLink(PublishController.Rels.BeginPublish, typeof(PublishController))]
    [HalActionLink(PublishController.Rels.Compile, typeof(PublishController))]

    [HalActionLink(HistoryController.Rels.ListHistory, typeof(HistoryController))]

    [HalActionLink(MergeController.Rels.GetMergeInfo, typeof(MergeController))]

    [HalActionLink(PageController.Rels.List, typeof(PageController))]

    [HalActionLink(TemplateController.Rels.List, typeof(TemplateController))]

    [HalActionLink(UploadController.Rels.UploadFile, typeof(UploadController))]
    [HalActionLink(UploadController.Rels.DeleteFile, typeof(UploadController))]
    [HalActionLink(UploadController.Rels.ListUploadedFiles, typeof(UploadController))]

    [HalActionLink(AssetController.Rels.AddAsset, typeof(AssetController))]

    [HalActionLink(typeof(BranchController), nameof(BranchController.List))]
    [HalActionLink(typeof(BranchController), nameof(BranchController.Add))]
    public class EntryPoint
    {
    }
}
