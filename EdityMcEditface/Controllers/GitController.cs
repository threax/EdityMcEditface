using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LibGit2Sharp;
using Microsoft.AspNetCore.StaticFiles;
using System.IO;
using EdityMcEditface.Models.Git;

namespace GitService.Controllers
{
    [Route("edity/[controller]/[action]")]
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
                return new UncommittedChange()
                {
                    State = s.State,
                    FilePath = s.FilePath
                };
            });
        }

        [HttpGet]
        public IEnumerable<History> History()
        {
            var historyCommits = repo.Commits;
            foreach (var logEntry in historyCommits)
            {
                yield return new History()
                {
                    Message = logEntry.Message,
                    Name = logEntry.Author.Name,
                    Email = logEntry.Author.Email,
                    When = logEntry.Author.When,
                    Sha = logEntry.Sha
                };
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
        public void Commit([FromBody]NewCommit newCommit)
        {
            var status = repo.RetrieveStatus();

            if (status.IsDirty)
            {
                repo.Stage("*");
                var signature = new Signature("Andrew Piper", "piper.andrew@spcollege.edu", DateTime.Now);
                repo.Commit(newCommit.Message, signature, signature);
            }
        }
    }
}
