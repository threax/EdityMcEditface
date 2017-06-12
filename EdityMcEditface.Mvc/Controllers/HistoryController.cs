using EdityMcEditface.HtmlRenderer.FileInfo;
using EdityMcEditface.Mvc.Models;
using EdityMcEditface.Mvc.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Threax.AspNetCore.Halcyon.Ext;

namespace EdityMcEditface.Mvc.Controllers
{
    [Route("edity/[controller]")]
    [ResponseCache(NoStore = true)]
    [ProducesHal]
    [TypeFilter(typeof(HalModelResultFilterAttribute))]
    [Authorize(Roles = Roles.EditPages)]
    public class HistoryController : Controller
    {
        public static class Rels
        {
            public const String ListHistory = "ListHistory";
            public const String ListPageHistory = "ListPageHistory";
        }

        private IHistoryRepository historyRepo;

        public HistoryController(IHistoryRepository historyRepo)
        {
            this.historyRepo = historyRepo;
        }

        /// <summary>
        /// Get the history of a particular page or file.
        /// </summary>
        /// <param name="query">The paged collection query to get the values.</param>
        /// <returns></returns>
        [HttpGet]
        [HalRel(Rels.ListHistory)]
        public Task<HistoryCollection> Get([FromQuery] HistoryQuery query)
        {
            return historyRepo.PageHistory(query);
        }

        /// <summary>
        /// Get the history of a particular page or file.
        /// </summary>
        /// <param name="filePath">The path to the page to get history for.</param>
        /// <param name="query">The paged collection query to get the values.</param>
        /// <returns></returns>
        [HttpGet("[action]/{*FilePath}")]
        [HalRel(Rels.ListPageHistory)]
        public Task<HistoryCollection> Get(String filePath, [FromQuery] HistoryQuery query)
        {
            query.FilePath = filePath;
            return Get(query);
        }
    }
}
