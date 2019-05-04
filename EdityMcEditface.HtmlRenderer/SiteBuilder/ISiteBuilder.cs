using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer.SiteBuilder
{
    public interface ISiteBuilder
    {
        /// <summary>
        /// Add a task that runs before build.
        /// </summary>
        /// <param name="task">The task to add.</param>
        void AddPreBuildTask(IBuildTask task);

        /// <summary>
        /// Add a task that runs after the build.
        /// </summary>
        /// <param name="task">The task to add.</param>
        void AddPostBuildTask(IBuildTask task);

        /// <summary>
        /// Add a task that does publishing steps. This phase will run after the build phases.
        /// All files that you wish to publish should have been copied by this point during the
        /// build phases.
        /// </summary>
        /// <param name="task">The task to add.</param>
        void AddPublishTask(IBuildTask task);

        /// <summary>
        /// Add a task runs after publishing is complete. This will run after PublishTasks. If you
        /// need to communicate with another service that expects all files to be copied to the
        /// published output add it here.
        /// </summary>
        /// <param name="task">The task to add.</param>
        void AddPostPublishTask(IBuildTask task);

        /// <summary>
        /// Build the site. This will run the PreBuild Tasks, then the main site build then the PostBuild
        /// tasks and then the Publish tasks.
        /// </summary>
        Task BuildSite(BuilderUserInfo userInfo);

        /// <summary>
        /// Open a read stream to a file in the source folder.
        /// </summary>
        /// <param name="file">The name of the file to open.</param>
        /// <returns>A stream to the file.</returns>
        Stream OpenInputReadStream(string file);

        /// <summary>
        /// Open a write stream to file in the destination folder.
        /// </summary>
        /// <param name="file">The file to write to.</param>
        /// <returns>A stream to the file.</returns>
        Stream OpenOutputWriteStream(String file);

        /// <summary>
        /// Open a write stream to a file in the parent destination folder. The meaning of this folder is different depending
        /// on the publish mode. If the publish mode is Direct this will be the same as the normal output folder. If the publish
        /// mode is RoundRobin this will be the parent folder that the round robin folder is going into.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        Stream OpenOutputParentWriteStream(string file);

        /// <summary>
        /// Determine if a given file exists in the site output.
        /// </summary>
        /// <param name="file">The file to check.</param>
        /// <returns>True if the output contains the file, false otherwise.</returns>
        bool DoesOutputFileExist(String file);

        /// <summary>
        /// Get the current progress of the build. The object returned will be thread safe.
        /// </summary>
        /// <returns></returns>
        BuildProgress GetCurrentProgress();

        /// <summary>
        /// If the builder is outputting to a subfolder of the output directory, such as in the Round Robin deployment
        /// method. This will return the name of that folder, otherwise it will be null.
        /// </summary>
        String DeploymentSubFolder { get; }

        /// <summary>
        /// The edity project settings for this Site Builder.
        /// </summary>
        EdityProject Project { get; }

        /// <summary>
        /// The settings for this site builder.
        /// </summary>
        SiteBuilderSettings Settings { get; }
    }
}
