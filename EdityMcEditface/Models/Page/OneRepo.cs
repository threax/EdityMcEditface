using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.Models.Page
{
    public class OneRepo : ProjectFinder
    {
        String projectFolder;

        public OneRepo(String projectFolder, String backupPath)
        {
            this.projectFolder = projectFolder;
            this.BackupPath = backupPath;
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

        public String BackupPath { get; private set; }
    }
}
