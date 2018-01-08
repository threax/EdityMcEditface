using EdityMcEditface.Mvc.Models.Phase;
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
    }
}
