using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibGit2Sharp;

namespace EdityMcEditface.Models.Git
{
    public class FileHistory
    {
        public string Message { get; internal set; }
        public string Sha { get; internal set; }
        public string Name { get; internal set; }
        public string Email { get; internal set; }
        public DateTimeOffset When { get; internal set; }
    }
}
