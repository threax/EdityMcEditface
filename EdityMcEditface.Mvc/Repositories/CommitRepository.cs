using EdityMcEditface.HtmlRenderer;
using EdityMcEditface.HtmlRenderer.FileInfo;
using EdityMcEditface.Mvc.Models.Git;
using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdityMcEditface.Mvc.Repositories
{
    public class CommitRepository : ICommitRepository
    {
        private Repository repo;
        private IFileFinder fileFinder;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="repo">The repo the commits live in.</param>
        /// <param name="fileFinder">The file finder to lookup files with.</param>
        public CommitRepository(Repository repo, IFileFinder fileFinder)
        {
            this.repo = repo;
            this.fileFinder = fileFinder;
        }

        /// <summary>
        /// Get an enumerable over all uncommitted changes.
        /// </summary>
        /// <returns></returns>
        public UncommittedChangeCollection UncommittedChanges()
        {
            return new UncommittedChangeCollection(QueryChanges(repo.RetrieveStatus()).Select(s =>
            {
                return new UncommittedChange(s.State)
                {
                    FilePath = s.FilePath
                };
            }));
        }

        /// <summary>
        /// Check to see if there are any uncommitted changes.
        /// </summary>
        /// <returns>True if there are uncommitted changes, false otherwise.</returns>
        public bool HasUncommittedChanges()
        {
            return QueryChanges(repo.RetrieveStatus()).Any();
        }

        /// <summary>
        /// Get the diff of the file between its original and modified version.
        /// </summary>
        /// <param name="fileInfo">The file info for the file to get a diff for.</param>
        /// <returns>The file's diff info.</returns>
        public DiffInfo UncommittedDiff(ITargetFileInfo fileInfo)
        {
            if (!IsWritablePath(fileInfo.DerivedFileName))
            {
                throw new FileNotFoundException($"Cannot access file '{fileInfo.OriginalFileName}'");
            }

            var diff = new DiffInfo();

            //Original File
            var historyCommits = repo.Commits.QueryBy(fileInfo.DerivedFileName);
            var latestCommit = historyCommits.FirstOrDefault();
            if (latestCommit != null)
            {
                var blob = latestCommit.Commit[fileInfo.DerivedFileName].Target as Blob;
                if (blob != null)
                {
                    diff.Original = blob.GetContentText();
                }
            }
            else
            {
                diff.Original = $"Could not read original file {fileInfo.DerivedFileName}.";
            }

            var repoPath = Path.Combine(repo.Info.WorkingDirectory, fileInfo.DerivedFileName);
            using (var stream = new StreamReader(fileFinder.ReadFile(fileFinder.GetProjectRelativePath(repoPath))))
            {
                diff.Changed = stream.ReadToEnd().Replace("\r", "");
            }

            return diff;
        }

        /// <summary>
        /// Commit the current working directory.
        /// </summary>
        /// <param name="signature">The user signature, from services.</param>
        /// <param name="newCommit">The new commit object.</param>
        public void Commit(Signature signature, NewCommit newCommit)
        {
            if (!repo.Index.IsFullyMerged)
            {
                throw new InvalidOperationException("Cannot commit while there are conflicts. Please resolve these first.");
            }

            var status = repo.RetrieveStatus();

            if (status.IsDirty)
            {
                foreach (var path in QueryChanges(status).Select(s => s.FilePath))
                {
                    Commands.Stage(repo, path);
                }
                repo.Commit(newCommit.Message, signature, signature);
            }
        }

        /// <summary>
        /// Revert a file to its unmodified version.
        /// </summary>
        /// <param name="fileInfo">The file info for the file to revert.</param>
        /// <returns></returns>
        public async Task Revert(ITargetFileInfo fileInfo)
        {
            if (!IsWritablePath(fileInfo.DerivedFileName))
            {
                throw new FileNotFoundException($"Cannot access file '{fileInfo.OriginalFileName}'");
            }

            //Original File
            var historyCommits = repo.Commits.QueryBy(fileInfo.DerivedFileName);
            var latestCommit = historyCommits.FirstOrDefault();
            if (latestCommit != null)
            {
                var blob = latestCommit.Commit[fileInfo.DerivedFileName].Target as Blob;
                if (blob != null)
                {
                    //Changed file
                    var repoPath = Path.Combine(repo.Info.WorkingDirectory, fileInfo.DerivedFileName);
                    using (var dest = fileFinder.WriteFile(fileFinder.GetProjectRelativePath(repoPath)))
                    {
                        using (var source = blob.GetContentStream())
                        {
                            await source.CopyToAsync(dest);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Helper function to query changes in the git repo.
        /// </summary>
        /// <param name="status">The status of the repository.</param>
        /// <returns></returns>
        private IEnumerable<StatusEntry> QueryChanges(RepositoryStatus status)
        {
            return status.Where(s => s.State != FileStatus.Ignored && IsWritablePath(s.FilePath));
        }

        /// <summary>
        /// Make sure the path is writable.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private bool IsWritablePath(String path)
        {
            return fileFinder.IsValidWritablePath(Path.Combine(repo.Info.WorkingDirectory, path));
        }
    }
}
