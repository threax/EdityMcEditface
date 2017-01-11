using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LibGit2Sharp;
using Microsoft.AspNetCore.StaticFiles;
using System.IO;
using EdityMcEditface.Mvc.Models.Git;
using Microsoft.AspNetCore.Authorization;
using EdityMcEditface.HtmlRenderer;
using System.Net;
using Threax.AspNetCore.ExceptionToJson;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace EdityMcEditface.Mvc.Controllers
{
    /// <summary>
    /// This api interacts with the git repo.
    /// </summary>
    [Route("edity/[controller]/[action]")]
    [Authorize(Roles = Roles.EditPages)]
    public class GitController : Controller
    {
        private Repository repo;
        private FileFinder fileFinder;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="fileFinder"></param>
        public GitController(Repository repo, FileFinder fileFinder)
        {
            this.repo = repo;
            this.fileFinder = fileFinder;
        }

        /// <summary>
        /// Dispose function.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            this.repo.Dispose();
            base.Dispose(disposing);
        }

        /// <summary>
        /// Get the uncommitted changes.
        /// </summary>
        /// <returns>The uncommitted changes.</returns>
        [HttpGet]
        public IEnumerable<UncommittedChange> UncommittedChanges()
        {
            return QueryChanges(repo.RetrieveStatus()).Select(s =>
            {
                return new UncommittedChange(s.State)
                {
                    FilePath = s.FilePath
                };
            });
        }

        /// <summary>
        /// Get the files that need to be synced.
        /// </summary>
        /// <returns>A SyncInfo with the info.</returns>
        [HttpGet]
        public async Task<SyncInfo> SyncInfo()
        {
            return await Task.Run<SyncInfo>(() =>
            {
                repo.Fetch("origin");

                var head = repo.Head.Commits.First();
                var tracked = repo.Head.TrackedBranch.Commits.First();
                var divergence = repo.ObjectDatabase.CalculateHistoryDivergence(head, tracked);

                var aheadCommits = repo.Commits.QueryBy(new CommitFilter()
                {
                    SortBy = CommitSortStrategies.Reverse | CommitSortStrategies.Time,
                    IncludeReachableFrom = divergence.One,
                    ExcludeReachableFrom = divergence.CommonAncestor
                });

                var behindCommits = repo.Commits.QueryBy(new CommitFilter()
                {
                    SortBy = CommitSortStrategies.Reverse | CommitSortStrategies.Time,
                    IncludeReachableFrom = divergence.Another,
                    ExcludeReachableFrom = divergence.CommonAncestor
                });

                return new SyncInfo()
                {
                    HasUncomittedChanges = UncommittedChanges().Any(),
                    AheadBy = divergence.AheadBy.GetValueOrDefault(),
                    BehindBy = divergence.BehindBy.GetValueOrDefault(),
                    AheadHistory = aheadCommits.Select(c => new History(c)),
                    BehindHistory = behindCommits.Select(c => new History(c))
                };
            });
        }

        /// <summary>
        /// Get the diff of the file between its original and modified version.
        /// </summary>
        /// <param name="file">The file to get a diff for.</param>
        /// <returns>The file's diff info.</returns>
        [HttpGet]
        public DiffInfo UncommittedDiff([FromQuery] String file)
        {
            if (!IsWritablePath(file))
            {
                throw new ErrorResultException($"Cannot access file '{file}'");
            }

            var diff = new DiffInfo();

            //Original File
            var historyCommits = repo.Commits.QueryBy(file);
            var latestCommit = historyCommits.FirstOrDefault();
            if (latestCommit != null)
            {
                var blob = latestCommit.Commit[file].Target as Blob;
                if (blob != null)
                {
                    diff.Original = blob.GetContentText();
                }
            }
            else
            {
                diff.Original = $"Could not read original file {file}.";
            }

            //Changed file
            var targetFileInfo = new TargetFileInfo(file);

            var repoPath = Path.Combine(repo.Info.WorkingDirectory, targetFileInfo.DerivedFileName);
            using (var stream = new StreamReader(fileFinder.readFile(fileFinder.getProjectRelativePath(repoPath))))
            {
                diff.Changed = stream.ReadToEnd().Replace("\r", "");
            }

            return diff;
        }

        /// <summary>
        /// Get the history of the entire repo.
        /// </summary>
        /// <param name="page">The page to lookup, defaults to 0.</param>
        /// <param name="count">The number of pages to return, defaults to 25.</param>
        /// <returns>The history of the page.</returns>
        [HttpGet]
        public IEnumerable<History> RepoHistory([FromQuery]int page = 0, [FromQuery]int count = 25)
        {
            var historyCommits = repo.Commits.Skip(page * count).Take(count);
            foreach (var logEntry in historyCommits)
            {
                yield return new History(logEntry);
            }
        }

        /// <summary>
        /// Get the number of history entries for the entire repo.
        /// </summary>
        /// <param name="file">The file to lookup.</param>
        /// <returns>The number of history entries for the file.</returns>
        [HttpGet]
        public int RepoHistoryCount([FromQuery] String file)
        {
            if (!IsWritablePath(file))
            {
                throw new ErrorResultException($"Cannot access file '{file}'");
            }

            var fileInfo = new TargetFileInfo(file);

            return repo.Commits.QueryBy(fileInfo.DerivedFileName).Count();
        }

        /// <summary>
        /// The conflicts in the current working directory.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<ConflictInfo> Conflicts()
        {
            return repo.Index.Conflicts.Select(s =>
            {
                return new ConflictInfo()
                {
                    FilePath = s.Ancestor.Path
                };
            });
        }

        /// <summary>
        /// Get the info for a merge for a file.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpGet]
        public MergeInfo MergeInfo([FromQuery] String file)
        {
            if (!IsWritablePath(file))
            {
                throw new ErrorResultException($"Cannot access file '{file}'");
            }

            var targetFileInfo = new TargetFileInfo(file);

            var repoPath = Path.Combine(repo.Info.WorkingDirectory, targetFileInfo.DerivedFileName);
            using (var stream = new StreamReader(fileFinder.readFile(fileFinder.getProjectRelativePath(repoPath))))
            {
                return new MergeInfo(stream);
            }
        }

        /// <summary>
        /// Ge the history of a file.
        /// </summary>
        /// <param name="file">The file to lookup.</param>
        /// <param name="page">The page to lookup, defaults to 0.</param>
        /// <param name="count">The number of pages to return, defaults to 25.</param>
        /// <returns>The history of the file.</returns>
        [HttpGet]
        public IEnumerable<History> FileHistory([FromQuery] String file, [FromQuery]int page = 0, [FromQuery]int count = 25)
        {
            if (!IsWritablePath(file))
            {
                throw new ErrorResultException($"Cannot access file '{file}'");
            }

            var fileInfo = new TargetFileInfo(file);

            var historyCommits = repo.Commits.QueryBy(fileInfo.DerivedFileName).Skip(page * count).Take(count);
            foreach (var logEntry in historyCommits)
            {
                yield return new History()
                {
                    Message = logEntry.Commit.Message,
                    Name = logEntry.Commit.Author.Name,
                    Email = logEntry.Commit.Author.Email,
                    When = logEntry.Commit.Author.When,
                    Sha = logEntry.Commit.Sha
                };
            }
        }

        /// <summary>
        /// Get a particluar file version.
        /// </summary>
        /// <param name="sha">The sha to lookup.</param>
        /// <param name="file">The file.</param>
        /// <returns>The file for the given sha version.</returns>
        [HttpGet]
        public FileStreamResult FileVersion([FromQuery] String sha, [FromQuery] String file)
        {
            if (!IsWritablePath(file))
            {
                throw new ErrorResultException($"Cannot access file '{file}'");
            }

            var contentTypeProvider = new FileExtensionContentTypeProvider();
            String contentType;
            if (contentTypeProvider.TryGetContentType(file, out contentType))
            {
                var commit = repo.Lookup<Commit>(sha);
                var treeEntry = commit[file];
                var blob = treeEntry.Target as Blob;
                return File(blob.GetContentStream(), contentType);
            }
            else
            {
                throw new NotSupportedException($"Files with extension {Path.GetExtension(file)} not supported.");
            }
        }

        /// <summary>
        /// Commit the current working directory.
        /// </summary>
        /// <param name="signature">The user signature, from services.</param>
        /// <param name="newCommit">The new commit object.</param>
        [HttpPost]
        public void Commit([FromServices]Signature signature, [FromBody]NewCommit newCommit)
        {
            if (!repo.Index.IsFullyMerged)
            {
                throw new ErrorResultException("Cannot commit while there are conflicts. Please resolve these first.");
            }

            var status = repo.RetrieveStatus();

            if (status.IsDirty)
            {
                foreach(var path in QueryChanges(status).Select(s => s.FilePath))
                {
                    repo.Stage(path);
                }
                repo.Commit(newCommit.Message, signature, signature);
            }
        }

        /// <summary>
        /// Pull in changes from the origin repo.
        /// </summary>
        /// <param name="signature">The signature to use, from services.</param>
        [HttpPost]
        public void Pull([FromServices]Signature signature)
        {
            if (UncommittedChanges().Any())
            {
                throw new ErrorResultException("Cannot pull with uncommitted changes. Please commit first and try again.");
            }

            try
            {
                var result = repo.Network.Pull(signature, new PullOptions());
                switch (result.Status)
                {
                    case MergeStatus.Conflicts:
                        throw new ErrorResultException("Pull from source resulted in conflicts, please resolve them manually.");
                }
            }
            catch (LibGit2SharpException ex)
            {
                throw new ErrorResultException(ex.Message);
            }
        }

        /// <summary>
        /// Push changes to the origin repo.
        /// </summary>
        [HttpPost]
        public async Task Push()
        {
            if (UncommittedChanges().Any())
            {
                throw new ErrorResultException("Cannot push with uncommitted changes. Please commit first and try again.");
            }

            try
            {
                repo.Network.Push(repo.Head, null);
            }
            catch (LibGit2SharpException ex)
            {
                throw new ErrorResultException(ex.Message);
            }

            //Garbage collect using git gc, if it is not installed this will silently fail
            try
            {
                await Task.Run(() =>
                {
                    var p = Process.Start( new ProcessStartInfo() {
                        FileName = "git",
                        Arguments = "gc --auto",
                        CreateNoWindow = true
                    });
                    p.WaitForExit(300000); //Wait for 5 minutes, may need adjustment
                });
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// Resolve the conflicts on the file.
        /// </summary>
        /// <param name="file">The file to resolve.</param>
        /// <param name="content">The file content.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task Resolve([FromQuery] String file, IFormFile content)
        {
            if (!IsWritablePath(file))
            {
                throw new ErrorResultException($"Cannot access file '{file}'");
            }

            if (!repo.Index.Conflicts.Any(s => file == s.Ancestor.Path))
            {
                throw new ErrorResultException($"No conflicts to resolve for {file}.");
            }

            TargetFileInfo fileInfo = new TargetFileInfo(file);
            var repoPath = Path.Combine(repo.Info.WorkingDirectory, fileInfo.DerivedFileName);
            using (var stream = fileFinder.writeFile(fileFinder.getProjectRelativePath(repoPath)))
            {
                await content.CopyToAsync(stream);
            }

            repo.Index.Add(file);
        }

        /// <summary>
        /// Revert a file to its unmodified version.
        /// </summary>
        /// <param name="file">The file to revert.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task Revert([FromQuery] String file)
        {
            if (!IsWritablePath(file))
            {
                throw new ErrorResultException($"Cannot access file '{file}'");
            }

            //Original File
            var historyCommits = repo.Commits.QueryBy(file);
            var latestCommit = historyCommits.FirstOrDefault();
            if (latestCommit != null)
            {
                var blob = latestCommit.Commit[file].Target as Blob;
                if (blob != null)
                {
                    //Changed file
                    var targetFileInfo = new TargetFileInfo(file);
                    var repoPath = Path.Combine(repo.Info.WorkingDirectory, targetFileInfo.DerivedFileName);
                    using (var dest = fileFinder.writeFile(fileFinder.getProjectRelativePath(repoPath)))
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
            return fileFinder.isValidWritablePath(Path.Combine(repo.Info.WorkingDirectory, path));
        }
    }
}
