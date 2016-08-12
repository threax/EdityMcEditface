using EdityMcEditface.HtmlRenderer;
using EdityMcEditface.Models.Auth;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EdityMcEditface.Models.Page
{
    public interface ProjectFinder
    {
        String GetUserProjectPath(String user);

        String PublishedProjectPath { get; }

        String MasterRepoPath { get; }

        String BackupPath { get; }
    }
}
