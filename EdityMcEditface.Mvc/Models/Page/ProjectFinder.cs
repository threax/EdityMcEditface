using System;

namespace EdityMcEditface.Mvc.Models.Page
{
    public interface ProjectFinder
    {
        String GetUserProjectPath(String user);

        String PublishedProjectPath { get; }

        String MasterRepoPath { get; }

        String BackupPath { get; }
    }
}
