using System;
using System.Collections.Generic;
using System.Text;

namespace EdityMcEditface.Mvc.Config
{
    /// <summary>
    /// The different modes to run the project in.
    /// </summary>
    public enum ProjectMode
    {
        /// <summary>
        /// One shared repo for all users. Best for single user or local configurations.
        /// </summary>
        OneRepo = 0,

        /// <summary>
        /// One repo per user of the system. Creates a central repo that can be synced between and one repo for each user.
        /// </summary>
        OneRepoPerUser = 1
    }
}
