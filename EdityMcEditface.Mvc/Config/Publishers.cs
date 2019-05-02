using System;
using System.Collections.Generic;
using System.Text;

namespace EdityMcEditface.Mvc.Config
{
    /// <summary>
    /// The different publishers.
    /// </summary>
    public enum Publishers
    {
        /// <summary>
        /// Directly publish the website to a target folder.
        /// </summary>
        Direct = 0,

        /// <summary>
        /// Publish the website to folder, but put the content in a subdirectory. This subdirectory is a guid. The webserver
        /// should map the urls so that they go into the subdirectory, but keep that from being publicly accessible. This
        /// keeps the website running while it is compiling so there is less downtime.
        /// </summary>
        RoundRobin = 1
    }
}
