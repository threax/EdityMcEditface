using EdityMcEditface.HtmlRenderer.Filesystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace EdityMcEditface.Mvc.Models.Page
{
    public class MustHaveGitDraftFile : PathPermissionChain
    {
        private GitDraftManager draftManager;

        public MustHaveGitDraftFile(IDraftManager draftManager, IPathPermissions next = null)
            : base(next)
        {
            this.draftManager = draftManager as GitDraftManager;
            if (this.draftManager == null)
            {
                throw new InvalidCastException("Cannot cast the IDraftManager provided to a GitDraftManager when creating a MustHaveGitDraftFile. Please make sure your custom draft manager is a subclass of GitDraftManager or use a different FileStreamManager implementation");
            }
        }

        public override bool AllowFile(string path, String physicalPath)
        {
            bool valid = true;
            if (draftManager.IsDraftedFile(physicalPath))
            {
                var draftInfo = draftManager.LoadDraftInfo(physicalPath);
                valid = draftInfo.Sha != null;
            }
            return valid && this.Next(path, physicalPath);
        }
    }
}
