using System.Collections.Generic;
using System.IO;
using EdityMcEditface.Mvc.Models.Git;
using Threax.AspNetCore.Halcyon.Ext;
using EdityMcEditface.Mvc.Models;
using EdityMcEditface.HtmlRenderer.FileInfo;
using System.Threading.Tasks;

namespace EdityMcEditface.Mvc.Repositories
{
    public interface IHistoryRepository
    {
        /// <summary>
        /// Get the history of a page, will include history of all files that make the page.
        /// </summary>
        /// <param name="query">The paged query.</param>
        /// <returns>The history of the page.</returns>
        Task<HistoryCollection> PageHistory(HistoryQuery query);
    }
}