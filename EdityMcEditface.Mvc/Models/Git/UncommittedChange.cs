using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibGit2Sharp;

namespace EdityMcEditface.Models.Git
{
    /// <summary>
    /// A verision of FileStatus from git with ambiguity removed
    /// </summary>
    public enum GitFileStatus
    {
        Nonexistent,
        Unaltered,
        Added,
        Removed,
        Renamed,
        Modified,
        Unreadable,
        Ignored,
        Conflicted,
    }

    public class UncommittedChange
    {
        public UncommittedChange()
        {

        }

        public UncommittedChange(FileStatus status)
        {
            setStatusFromFileStatus(status);
        }

        public string FilePath { get; internal set; }

        public GitFileStatus State { get; internal set; }

        public void setStatusFromFileStatus(FileStatus status)
        {
            switch (status)
            {
                case FileStatus.Nonexistent:
                    State = GitFileStatus.Nonexistent;
                    break;
                case FileStatus.Unaltered:
                    State = GitFileStatus.Unaltered;
                    break;
                case FileStatus.NewInIndex:
                case FileStatus.NewInWorkdir:
                    State = GitFileStatus.Added;
                    break;
                case FileStatus.DeletedFromIndex:
                case FileStatus.DeletedFromWorkdir:
                    State = GitFileStatus.Removed;
                    break;
                case FileStatus.RenamedInIndex:
                case FileStatus.RenamedInWorkdir:
                    State = GitFileStatus.Renamed;
                    break;
                case FileStatus.TypeChangeInIndex:
                case FileStatus.TypeChangeInWorkdir:
                case FileStatus.ModifiedInIndex:
                case FileStatus.ModifiedInWorkdir:
                    State = GitFileStatus.Modified;
                    break;
                case FileStatus.Unreadable:
                    State = GitFileStatus.Unreadable;
                    break;
                case FileStatus.Ignored:
                    State = GitFileStatus.Ignored;
                    break;
                case FileStatus.Conflicted:
                    State = GitFileStatus.Conflicted;
                    break;
            }
        }
    }
}
