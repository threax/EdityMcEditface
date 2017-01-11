using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibGit2Sharp;

namespace EdityMcEditface.Mvc.Models.Git
{
    public class ConflictInfo
    {
        public string FilePath { get; internal set; }
    }
}
