using EdityMcEditface.HtmlRenderer.Filesystem;
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
    [HalSelfActionLink(DraftController.Rels.Get, typeof(DraftController))]
    [HalActionLink(DraftController.Rels.SubmitLatestDraft, typeof(DraftController))]
    [HalActionLink(DraftController.Rels.SubmitAllDrafts, typeof(DraftController))]
    [HalActionLink(HistoryController.Rels.ListPageHistory, typeof(HistoryController))]
    public class Draft
    {
        public Draft()
        {

        }

        public Draft(DraftInfo info, String title)
        {
            this.LastUpdate = info.LastUpdate;
            this.Status = info.Status;
            this.File = info.File;
            this.Title = title;
        }

        public DateTime? LastUpdate { get; set; }

        public DraftStatus Status { get; set; }

        public String File { get; set; }

        /// <summary>
        /// Use this to shim File over to FilePath, which is used by other controllers to see files
        /// </summary>
        [JsonIgnore]
        public String FilePath
        {
            get
            {
                return File;
            }
        }

        public String Title { get; set; }
    }
}
