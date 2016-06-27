using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LibGit2Sharp;
using Microsoft.AspNetCore.StaticFiles;
using System.IO;

namespace GitService.Controllers
{
    [Route("api/[controller]/[action]")]
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
        public IEnumerable<StatusEntry> UncomittedChanges()
        {
            var status = repo.RetrieveStatus();
            return status.Where(s => s.State != FileStatus.Ignored);
        }

        [HttpGet("{*file}")]
        public IEnumerable<Object> History(String file)
        {
            var historyCommits = repo.Commits.QueryBy(file);
            foreach (var logEntry in historyCommits)
            {
                yield return new
                {
                    message = logEntry.Commit.Message,
                    author = logEntry.Commit.Author,
                    sha = logEntry.Commit.Sha
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
    }
}
