using EdityMcEditface.Mvc.Models.Branch;
using System;
using System.Collections;
using System.Threading.Tasks;

namespace EdityMcEditface.Mvc.Models.Page
{
    public interface ProjectFinder
    {
        /// <summary>
        /// Get the project path per user.
        /// </summary>
        /// <param name="user">The current user name.</param>
        /// <returns>The path to the branch.</returns>
        String GetUserProjectPath(String user);

        /// <summary>
        /// Get the path to the publish repo.
        /// </summary>
        String PublishedProjectPath { get; }

        /// <summary>
        /// Specifies which git branch to use for the published content.
        /// Can be null to specify default.
        /// </summary>
        String PublishedBranch { get; }

        /// <summary>
        /// The path to the master / shared repo.
        /// </summary>
        String MasterRepoPath { get; }

        /// <summary>
        /// The site to the edity client side core files.
        /// </summary>
        String EdityCorePath { get; }

        /// <summary>
        /// The path to the site's files.
        /// </summary>
        String SitePath { get; }

        /// <summary>
        /// Get the branches for the current project.
        /// </summary>
        /// <returns></returns>
        Task<BranchViewCollection> GetBranches();
    }
}
