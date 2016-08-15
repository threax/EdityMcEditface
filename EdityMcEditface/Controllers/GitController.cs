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

        public GitController(Repository repo)
        {
            this.repo = repo;
        }

        protected override void Dispose(bool disposing)
        {
            this.repo.Dispose();
            base.Dispose(disposing);
        }

        [HttpGet]
        public IEnumerable<UncommittedChange> UncommittedChanges()
        {
            var status = repo.RetrieveStatus();

            return status.Where(s => s.State != FileStatus.Ignored).Select(s =>
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
        public DiffInfo UncommittedDiff([FromServices] FileFinder fileFinder, String file)
        {
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

            using (var stream = new StreamReader(fileFinder.readFile(targetFileInfo.DerivedFileName)))
            {
                diff.Changed = stream.ReadToEnd().Replace("\r", "");
            }

            return diff;
        }

        [HttpGet]
        public IEnumerable<History> History()
        {
            var historyCommits = repo.Commits;
            foreach (var logEntry in historyCommits)
            {
                yield return new History(logEntry);
            }
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
        public MergeInfo MergeInfo([FromServices] FileFinder fileFinder, String file)
        {
            var targetFileInfo = new TargetFileInfo(file);

            using (var stream = new StreamReader(fileFinder.readFile(targetFileInfo.DerivedFileName)))
            {
                return new MergeInfo(stream);
            }
        }

        [HttpGet("{*file}")]
        public IEnumerable<History> History(String file)
        {
            var fileInfo = new TargetFileInfo(file);

            var historyCommits = repo.Commits.QueryBy(fileInfo.DerivedFileName);
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
        public void Commit([FromServices]Signature signature, [FromBody]NewCommit newCommit)
        {
            if (!repo.Index.IsFullyMerged)
            {
                throw new ErrorResultException("Cannot commit while there are conflicts. Please resolve these first.");
            }

            var status = repo.RetrieveStatus();

            if (status.IsDirty)
            {
                repo.Stage("*");
                repo.Commit(newCommit.Message, signature, signature);
            }
        }

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

        [HttpPost]
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
        public async Task Resolve([FromServices] FileFinder fileFinder, String file)
        {
            if (repo.Index.Conflicts.Any(s => file == s.Ancestor.Path))
            {
                throw new ErrorResultException($"No conflicts to resolve for {file}.");
            }

            TargetFileInfo fileInfo = new TargetFileInfo(file);
            using (Stream stream = fileFinder.writeFile(fileInfo.OriginalFileName))
            {
                await this.Request.Form.Files.First().CopyToAsync(stream);
            }

            repo.Index.Add(file);
        }

        [HttpPost("{*file}")]
        public async Task Revert([FromServices] FileFinder fileFinder, String file)
        {
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

                    using (var dest = fileFinder.writeFile(targetFileInfo.DerivedFileName))
                    {
                        using (var source = blob.GetContentStream())
                        {
                            await source.CopyToAsync(dest);
                        }
                    }
                }
            }
        }
    }
}
