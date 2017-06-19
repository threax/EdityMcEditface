using EdityMcEditface.Mvc.Models.Assets;
using EdityMcEditface.Mvc.Models.Page;
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
    /// <summary>
    /// Controller to handle assets for a page.
    /// </summary>
    [Route("edity/[controller]")]
    [ResponseCache(NoStore = true)]
    [ProducesHal]
    [TypeFilter(typeof(HalModelResultFilterAttribute))]
    [Authorize(Roles = Roles.EditPages)]
    public class AssetController : Controller
    {
        public static class Rels
        {
            public const String AddAsset = "AddAsset";
        }

        private IAssetRepository assetRepo;

        public AssetController(IAssetRepository assetRepo)
        {
            this.assetRepo = assetRepo;
        }

        /// <summary>
        /// Add an asset to a page.
        /// </summary>
        /// <param name="arg">The file content.</param>
        /// <returns>The ImageUploadResponse with the result.</returns>
        [HalRel(Rels.AddAsset)]
        [HttpPost]
        public Task<ImageUploadResponse> Asset([FromForm] ImageUploadInput arg)
        {
            return assetRepo.AddAsset(arg);
        }
    }
}
