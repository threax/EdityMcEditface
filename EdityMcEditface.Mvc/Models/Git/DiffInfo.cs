using EdityMcEditface.Mvc.Controllers;
using Halcyon.HAL.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Threax.AspNetCore.Halcyon.Ext;

namespace EdityMcEditface.Mvc.Models.Git
{
    [HalModel]
    [HalSelfActionLink(CommitController.Rels.GetUncommittedDiff, typeof(CommitController))]
    [HalActionLink(HistoryController.Rels.ListPageHistory, typeof(HistoryController))]
    public class DiffInfo
    {
        public DiffInfo()
        {

        }

        public String FilePath { get; set; }

        public String Original { get; set; }

        public String Changed { get; set; }
    }
}
