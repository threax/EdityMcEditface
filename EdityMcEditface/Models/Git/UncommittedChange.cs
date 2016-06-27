using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibGit2Sharp;

namespace EdityMcEditface.Models.Git
{
    public class UncommittedChange
    {
        public string FilePath { get; internal set; }

        public FileStatus State { get; internal set; }
    }
}
