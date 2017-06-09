using System.Threading.Tasks;
using EdityMcEditface.Mvc.Models;

namespace EdityMcEditface.Mvc.Repositories
{
    /// <summary>
    /// The draft repository manages drafts for pages and files.
    /// </summary>
    public interface IDraftRepository
    {
        /// <summary>
        /// Get the draft info for a single file.
        /// </summary>
        /// <param name="file">The file to lookup.</param>
        /// <returns>The draft info for the file.</returns>
        Task<Draft> GetInfo(string file);

        /// <summary>
        /// Get the list of pages in draft.
        /// </summary>
        /// <returns>The list of drafted files.</returns>
        Task<DraftCollection> List(DraftQuery query);

        /// <summary>
        /// Mark the given file's latest revision as its draft.
        /// </summary>
        /// <param name="file">The name of the file.</param>
        /// <returns></returns>
        Task Submit(string file);

        /// <summary>
        /// Update all pages and files with never drafted out of date status to their latest draft.
        /// </summary>
        /// <returns></returns>
        Task SubmitAll();
    }
}