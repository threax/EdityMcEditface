using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdityMcEditface.Mvc.Models.Git
{
    public class MergeInfo
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

        public MergeInfo(TextReader content)
        {
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
    }
}
