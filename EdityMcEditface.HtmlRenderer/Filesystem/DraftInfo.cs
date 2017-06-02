using System;
using System.Collections.Generic;
using System.Text;

namespace EdityMcEditface.HtmlRenderer.Filesystem
{
    /// <summary>
    /// This enum captures the current status of a draft.
    /// </summary>
    public enum DraftStatus
    {
        /// <summary>
        /// This 
        /// </summary>
        UndraftedEdits,
        NeverDrafted,
        UpToDate
    }

    /// <summary>
    /// This class provides info about the current draft of a file.
    /// </summary>
    public class DraftInfo
    {
        public DraftInfo(DateTime? lastUpdate, DraftStatus status, String file)
        {
            this.LastUpdate = lastUpdate;
            this.Status = status;
            this.File = file;
        }

        public DateTime? LastUpdate { get; private set; }

        public DraftStatus Status { get; private set; }

        public String File { get; private set; }
    }
}
