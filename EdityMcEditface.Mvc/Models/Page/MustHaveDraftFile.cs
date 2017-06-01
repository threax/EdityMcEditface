using EdityMcEditface.HtmlRenderer.Filesystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace EdityMcEditface.Mvc.Models.Page
{
    public class MustHaveDraftFile : PathPermissionChain
    {
        private GitDraftManager draftManager;

        public MustHaveDraftFile(GitDraftManager draftManager, IPathPermissions next = null)
            : base(next)
        {
            this.draftManager = draftManager;
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
