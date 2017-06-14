using EdityMcEditface.HtmlRenderer;
using EdityMcEditface.HtmlRenderer.FileInfo;
using EdityMcEditface.Mvc.Models.Git;
using EdityMcEditface.Mvc.Services;
using LibGit2Sharp;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdityMcEditface.Mvc.Repositories
{
    public class MergeRepository : IMergeRepository
    {
        private IFileFinder fileFinder;
        private ITargetFileInfoProvider fileInfoProvider;
        private String pathBase;
        private Repository repo;

        public MergeRepository(Repository repo, IFileFinder fileFinder, ITargetFileInfoProvider fileInfoProvider, IPathBaseInjector pathBaseInjector)
        {
            this.fileFinder = fileFinder;
            this.fileInfoProvider = fileInfoProvider;
            this.pathBase = pathBaseInjector.PathBase;
            this.repo = repo;
        }

        public MergeInfo MergeInfo(String file)
        {
            var targetFileInfo = fileInfoProvider.GetFileInfo(file, pathBase);

            var repoPath = Path.Combine(repo.Info.WorkingDirectory, targetFileInfo.DerivedFileName);
            using (var stream = new StreamReader(fileFinder.ReadFile(fileFinder.GetProjectRelativePath(repoPath))))
            {
                return new MergeInfo(stream, file);
            }
        }

        /// <summary>
        /// Resolve the conflicts on the file.
        /// </summary>
        /// <param name="file">The file to resolve.</param>
        /// <param name="content">The file content.</param>
        /// <returns></returns>
        public async Task Resolve(String file, IFormFile content)
        {
            if (!repo.Index.Conflicts.Any(s => file == s.Ancestor.Path))
            {
                throw new InvalidOperationException($"No conflicts to resolve for {file}.");
            }

            var fileInfo = fileInfoProvider.GetFileInfo(file, pathBase);
            var repoPath = Path.Combine(repo.Info.WorkingDirectory, fileInfo.DerivedFileName);
            using (var stream = fileFinder.WriteFile(fileFinder.GetProjectRelativePath(repoPath)))
            {
                await content.CopyToAsync(stream);
            }

            //Staging clears the conflict status
            Commands.Stage(repo, file);
        }
    }
}
