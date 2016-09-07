using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LibGit2Sharp;
using Microsoft.AspNetCore.StaticFiles;
using System.IO;
using EdityMcEditface.Models.Git;
using Microsoft.AspNetCore.Authorization;
using EdityMcEditface.ErrorHandling;
using EdityMcEditface.HtmlRenderer;
using System.Net;

namespace EdityMcEditface.Controllers
{
    [Route("edity/[controller]/[action]")]
    [Authorize(Roles = Roles.EditPages)]
    public class GitController : Controller
    {
        private Repository repo;
        private FileFinder fileFinder;

        public GitController(Repository repo, FileFinder fileFinder)
        {
            this.repo = repo;
            this.fileFinder = fileFinder;
        }

        protected override void Dispose(bool disposing)
        {
            this.repo.Dispose();
            base.Dispose(disposing);
        }

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

        [HttpGet("{*file}")]
        public DiffInfo UncommittedDiff(String file)
        {
            if (!isWritablePath(file))
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

        [HttpGet]
        public IEnumerable<History> History([FromQuery]int page = 0, [FromQuery]int count = 25)
        {
            var historyCommits = repo.Commits.Skip(page * count).Take(count);
            foreach (var logEntry in historyCommits)
            {
                yield return new History(logEntry);
            }
        }

        [HttpGet("{*file}")]
        public int HistoryCount(String file)
        {
            if (!isWritablePath(file))
            {
                throw new ErrorResultException($"Cannot access file '{file}'");
            }

            var fileInfo = new TargetFileInfo(file);

            return repo.Commits.QueryBy(fileInfo.DerivedFileName).Count();
        }

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

        [HttpGet("{*file}")]
        public MergeInfo MergeInfo(String file)
        {
            if (!isWritablePath(file))
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

        [HttpGet("{*file}")]
        public IEnumerable<History> History(String file, [FromQuery]int page = 0, [FromQuery]int count = 25)
        {
            if (!isWritablePath(file))
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

        [HttpGet("{sha}/{*file}")]
        public FileStreamResult FileVersion(String sha, String file)
        {
            if (!isWritablePath(file))
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

        [HttpPost]
        [ValidateAntiForgeryToken]
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

        [HttpPost]
        [ValidateAntiForgeryToken]
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public void Push()
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
        }

        [HttpPost("{*file}")]
        [ValidateAntiForgeryToken]
        public async Task Resolve(String file)
        {
            if (!isWritablePath(file))
            {
                throw new ErrorResultException($"Cannot access file '{file}'");
            }

            if (repo.Index.Conflicts.Any(s => file == s.Ancestor.Path))
            {
                throw new ErrorResultException($"No conflicts to resolve for {file}.");
            }

            TargetFileInfo fileInfo = new TargetFileInfo(file);
            var repoPath = Path.Combine(repo.Info.WorkingDirectory, fileInfo.DerivedFileName);
            using (var stream = fileFinder.writeFile(fileFinder.getProjectRelativePath(repoPath)))
            {
                await this.Request.Form.Files.First().CopyToAsync(stream);
            }

            repo.Index.Add(file);
        }

        [HttpPost("{*file}")]
        [ValidateAntiForgeryToken]
        public async Task Revert(String file)
        {
            if (!isWritablePath(file))
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

        private IEnumerable<StatusEntry> QueryChanges(RepositoryStatus status)
        {
            return status.Where(s => s.State != FileStatus.Ignored && isWritablePath(s.FilePath));
        }

        private bool isWritablePath(String path)
        {
            return fileFinder.isValidWritablePath(Path.Combine(repo.Info.WorkingDirectory, path));
        }
    }
}
