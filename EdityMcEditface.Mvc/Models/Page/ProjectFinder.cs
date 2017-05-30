using EdityMcEditface.Mvc.Models.Branch;
using System;
using System.Collections;
using System.Threading.Tasks;

namespace EdityMcEditface.Mvc.Models.Page
{
    public interface ProjectFinder
    {
        String GetUserProjectPath(String user);

        String PublishedProjectPath { get; }

        /// <summary>
        /// Specifies which git branch to use for the published content.
        /// Can be null to specify default.
        /// </summary>
        String PublishedBranch { get; }

        String MasterRepoPath { get; }

        String EdityCorePath { get; }

        String SitePath { get; }

        /// <summary>
        /// Get the branches for the current project.
        /// </summary>
        /// <returns></returns>
        Task<BranchViewCollection> GetBranches();
    }
}
