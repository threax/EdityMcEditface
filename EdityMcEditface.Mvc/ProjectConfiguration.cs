using EdityMcEditface.HtmlRenderer.SiteBuilder;
using EdityMcEditface.PublishTasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace EdityMcEditface.Mvc
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
        RoundRobin = 2
    }

    public class ProjectConfiguration
    {
        /// <summary>
        /// The mode to handle the project's files.
        /// </summary>
        public ProjectMode ProjectMode { get; set; } = ProjectMode.OneRepo;

        /// <summary>
        /// The publisher to use.
        /// </summary>
        public Publishers Publisher { get; set; } = Publishers.Direct;

        /// <summary>
        /// The output folder for published files.
        /// </summary>
        public String OutputPath { get; set; }

        /// <summary>
        /// The path to the project.
        /// </summary>
        public String ProjectPath { get; set; }

        /// <summary>
        /// The path to the edity core client side files.
        /// </summary>
        public String EdityCorePath { get; set; }

        /// <summary>
        /// The path to this site's client side files.
        /// </summary>
        public String SitePath { get; set; }

        /// <summary>
        /// The default page for the project. Defaults to "index".
        /// </summary>
        public String DefaultPage { get; set; } = "index";

        /// <summary>
        /// The override variables for the project. These variables take ultimate precidence over all other
        /// variables defined in edity.json files. Mostly useful to override variables when working on the code.
        /// Defaults to null, which means no overrides.
        /// </summary>
        public Dictionary<String, String> OverrideVars { get; set; } = null;
    }
}
