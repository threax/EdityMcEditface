using EdityMcEditface.Mvc.Models.Page;
using EdityMcEditface.Mvc.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public class PageController : Controller
    {
        public static class Rels
        {
            public const String GetSettings = "GetSettings";
            public const String UpdateSettings = "UpdateSettings";
            public const String Save = "SavePage";
            public const String Delete = "DeletePage";
            public const String List = "ListPages";
        }

        private IPageRepository pageRepo;

        public PageController(IPageRepository pageRepo)
        {
            this.pageRepo = pageRepo;
        }

        /// <summary>
        /// Get the list of pages in draft.
        /// </summary>
        /// <returns>The list of drafted files.</returns>
        [HttpGet]
        [HalRel(Rels.List)]
        public Task<PageInfoCollection> List([FromQuery]PageQuery query)
        {
            if (query.File == null)
            {
                //Check url query, if file was in it we are looking for the default file (index usually)
                var hasFile = HttpContext.Request.Query.Any(i => "file".Equals(i.Key, StringComparison.OrdinalIgnoreCase));
                if (hasFile)
                {
                    query.File = "";
                }
            }
            return pageRepo.List(query);
        }

        /// <summary>
        /// Get the current page settings.
        /// </summary>
        /// <param name="filePath">The name of the file to lookup.</param>
        /// <returns>The PageSettings for the file.</returns>
        [HttpGet("Settings/{*FilePath}")]
        [HalRel(Rels.GetSettings)]
        public PageSettings Settings(String filePath)
        {
            return pageRepo.GetSettings(filePath);
        }

        /// <summary>
        /// Update the settings for the page.
        /// </summary>
        /// <param name="filePath">The file path to the page to change settings.</param>
        /// <param name="settings">The page settings to set.</param>
        [HttpPut("Settings/{*FilePath}")]
        [AutoValidate("Cannot update page settings.")]
        [HalRel(Rels.UpdateSettings)]
        public void UpdateSettings(String filePath, [FromBody]PageSettings settings)
        {
            pageRepo.UpdateSettings(filePath, settings);
        }

        /// <summary>
        /// Save a page.
        /// </summary>
        /// <param name="filePath">The file to save.</param>
        /// <param name="arg">The file content.</param>
        [HttpPut("{*FilePath}")]
        [HalRel(Rels.Save)]
        public Task Save(String filePath, [FromForm] SavePageInput arg)
        {
            return pageRepo.Save(filePath, arg.Content);
        }

        /// <summary>
        /// Delete a page.
        /// </summary>
        /// <param name="filePath">The name of the page to delete.</param>
        [HttpDelete("{*FilePath}")]
        [HalRel(Rels.Delete)]
        public void Delete(String filePath)
        {
            this.pageRepo.Delete(filePath);
        }
    }
}
