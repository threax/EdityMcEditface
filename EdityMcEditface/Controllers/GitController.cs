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
        public SyncInfo SyncInfo()
        {
            repo.Fetch("origin");

            var head = repo.Head.Commits.First();
            var tracked = repo.Head.TrackedBranch.Commits.First();
            var divergence = repo.ObjectDatabase.CalculateHistoryDivergence(head, tracked);
            Console.WriteLine(divergence);

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
        }

        [HttpGet("{*file}")]
        public DiffInfo UncommittedDiff([FromServices] FileFinder fileFinder, String file)
        {
            var diff = new DiffInfo();

            //Original File
            var historyCommits = repo.Commits.QueryBy(file);
            var latestCommit = historyCommits.First();
            var blob = latestCommit.Commit[file].Target as Blob;
            if (blob != null)
            {
                diff.Original = blob.GetContentText();
            }

            //Changed file
            var targetFileInfo = new TargetFileInfo(file);

            var openFile = targetFileInfo.OriginalFileName;
            if (targetFileInfo.Extension == "")
            {
                openFile = targetFileInfo.HtmlFile;
            }

            using (var stream = new StreamReader(fileFinder.readFile(openFile)))
            {
                diff.Changed = stream.ReadToEnd();
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

            var openFile = targetFileInfo.OriginalFileName;
            if (targetFileInfo.Extension == "")
            {
                openFile = targetFileInfo.HtmlFile;
            }

            using (var stream = new StreamReader(fileFinder.readFile(openFile)))
            {
                return new MergeInfo(stream);
            }
        }

        [HttpGet("{*file}")]
        public IEnumerable<History> History(String file)
        {
            var historyCommits = repo.Commits.QueryBy(file);
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
        public async Task<IActionResult> Resolve([FromServices] FileFinder fileFinder, String file)
        {
            if(repo.Index.Conflicts.Any(s => file == s.Ancestor.Path))
            {
                TargetFileInfo fileInfo = new TargetFileInfo(file);
                using (Stream stream = fileFinder.writeFile(fileInfo.OriginalFileName))
                {
                    await this.Request.Form.Files.First().CopyToAsync(stream);
                }

                repo.Index.Add(file);

                return StatusCode((int)HttpStatusCode.OK);
            }
            else
            {
                return StatusCode((int)HttpStatusCode.BadRequest);
            }
        }
    }
}
