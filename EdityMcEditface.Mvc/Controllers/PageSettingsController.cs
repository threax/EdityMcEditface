using EdityMcEditface.Mvc.Models.Page;
using EdityMcEditface.Mvc.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Threax.AspNetCore.Halcyon.Ext;

namespace EdityMcEditface.Mvc.Controllers
{
    [Route("edity/[controller]")]
    [ResponseCache(NoStore = true)]
    [ProducesHal]
    [TypeFilter(typeof(HalModelResultFilterAttribute))]
    [Authorize(AuthenticationSchemes = AuthCoreSchemes.Bearer, Roles = Roles.EditPages)]
    public class PageSettingsController : Controller
    {
        public static class Rels
        {
            public const String GetSettings = "GetSettings";
            public const String UpdateSettings = "UpdateSettings";
        }

        private IPageRepository pageRepo;

        public PageSettingsController(IPageRepository pageRepo)
        {
            this.pageRepo = pageRepo;
        }

        /// <summary>
        /// Get the current page settings.
        /// </summary>
        /// <param name="filePath">The name of the file to lookup.</param>
        /// <returns>The PageSettings for the file.</returns>
        [HttpGet("{*FilePath}")]
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
        [HttpPut("{*FilePath}")]
        [AutoValidate("Cannot update page settings.")]
        [HalRel(Rels.UpdateSettings)]
        public void UpdateSettings(String filePath, [FromBody]PageSettings settings)
        {
            pageRepo.UpdateSettings(filePath, settings);
        }
    }
}
