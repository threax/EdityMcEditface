using EdityMcEditface.Mvc.Controllers;
using Halcyon.HAL.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Threax.AspNetCore.Halcyon.Ext;

namespace EdityMcEditface.Mvc.Models.Git
{
    [HalModel]
    [HalSelfActionLink(MergeController.Rels.GetMergeInfo, typeof(MergeController))]
    [HalActionLink(MergeController.Rels.Resolve, typeof(MergeController))]
    public class MergeInfo : IQueryStringProvider
    {
        [Flags]
        enum AppendMode
        {
            None = 0,
            Theirs = 1,
            Mine = 2,
            Merged = 4,
            All = Theirs | Mine | Merged
        }

        private AppendMode topHalfMode = AppendMode.Mine | AppendMode.Merged;
        private AppendMode bottomHalfMode = AppendMode.Theirs;

        public MergeInfo(TextReader content, String file = null)
        {
            this.File = file;

            AppendMode appendMode = AppendMode.All;
            String line;
            StringBuilder merged = new StringBuilder();
            StringBuilder theirs = new StringBuilder();
            StringBuilder mine = new StringBuilder();
            while ((line = content.ReadLine()) != null)
            {
                if (line.StartsWith("<<<<<<<"))
                {
                    appendMode = topHalfMode;
                }
                else if (line.StartsWith("======="))
                {
                    appendMode = bottomHalfMode;
                }
                else if(line.StartsWith(">>>>>>>"))
                {
                    appendMode = AppendMode.All;
                }
                else //This skips the control lines from the file.
                {
                    if((appendMode & AppendMode.Merged) != AppendMode.None)
                    {
                        merged.AppendLine(line);
                    }

                    if ((appendMode & AppendMode.Theirs) != AppendMode.None)
                    {
                        theirs.AppendLine(line);
                    }

                    if ((appendMode & AppendMode.Mine) != AppendMode.None)
                    {
                        mine.AppendLine(line);
                    }
                }
            }

            Merged = merged.ToString();
            Theirs = theirs.ToString();
            Mine = mine.ToString();
        }

        public String Merged { get; set; }

        public String Theirs { get; set; }

        public String Mine { get; set; }

        public String File { get; set; }

        public void AddQuery(string rel, QueryStringBuilder queryString)
        {
            if(File != null && rel == HalSelfActionLinkAttribute.SelfRelName)
            {
                queryString.AppendItem("file", File);
            }
        }
    }
}
