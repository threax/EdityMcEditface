using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.Mvc.Models.Page
{
    public class OneRepo : ProjectFinder
    {
        String projectFolder;

        public OneRepo(String projectFolder, String edityCorePath, String sitePath)
        {
            this.projectFolder = projectFolder;
            this.EdityCorePath = edityCorePath;
            this.SitePath = sitePath;
            this.MasterRepoPath = projectFolder;
        }

        public String GetUserProjectPath(String user)
        {
            return projectFolder;
        }

        public String PublishedProjectPath
        {
            get
            {
                return projectFolder;
            }
        }

        public String EdityCorePath { get; private set; }

        public String SitePath { get; private set; }

        public String MasterRepoPath { get; private set; }
    }
}
