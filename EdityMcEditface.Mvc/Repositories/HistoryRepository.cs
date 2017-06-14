using EdityMcEditface.HtmlRenderer.FileInfo;
using EdityMcEditface.Mvc.Models;
using EdityMcEditface.Mvc.Models.Git;
using EdityMcEditface.Mvc.Services;
using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Threax.AspNetCore.Halcyon.Ext;

namespace EdityMcEditface.Mvc.Repositories
{
    public class HistoryRepository : IHistoryRepository
    {
        private Repository repo;
        private ITargetFileInfoProvider fileInfoProvider;
        private String pathBase;

        public HistoryRepository(Repository repo, ITargetFileInfoProvider fileInfoProvider, IPathBaseInjector pathBaseInjector)
        {
            this.repo = repo;
            this.fileInfoProvider = fileInfoProvider;
            this.pathBase = pathBaseInjector.PathBase;
        }

        public Task<HistoryCollection> PageHistory(HistoryQuery query)
        {
            var fileInfo = this.fileInfoProvider.GetFileInfo(query.FilePath, pathBase);

            var repoQuery = repo.Commits.QueryBy(fileInfo.DerivedFileName);
            var total = repoQuery.Count();

            var items = repoQuery
                .Skip(query.SkipTo(total)).Take(query.Limit)
                .Select(i => new History(i.Commit));

            return Task.FromResult(new HistoryCollection(query, total, items));
        }
    }
}
