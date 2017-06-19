using EdityMcEditface.HtmlRenderer;
using EdityMcEditface.Mvc.Models.Assets;
using EdityMcEditface.Mvc.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace EdityMcEditface.Mvc.Repositories
{
    public class AssetRepository : IAssetRepository
    {
        private IFileFinder fileFinder;
        private String pathBase;

        public AssetRepository(IFileFinder fileFinder, IPathBaseInjector pathBaseInjector)
        {
            this.fileFinder = fileFinder;
            this.pathBase = pathBaseInjector.PathBase;
        }

        public async Task<ImageUploadResponse> AddAsset(ImageUploadInput arg)
        {
            ImageUploadResponse imageResponse = new ImageUploadResponse();

            try
            {
                string autoFileFolder = "AutoUploads";
                var autoFileFile = Guid.NewGuid().ToString() + Path.GetExtension(arg.Upload.FileName);
                var autoPath = Path.Combine(autoFileFolder, autoFileFile);
                using (Stream stream = fileFinder.WriteFile(autoPath))
                {
                    await arg.Upload.CopyToAsync(stream);
                }

                imageResponse.Uploaded = 1;
                imageResponse.FileName = autoFileFile;
                imageResponse.Url = pathBase + autoPath.EnsureStartingPathSlash();
            }
            catch (Exception ex)
            {
                imageResponse.Message = ex.Message;
                imageResponse.Uploaded = 0;
            }

            return imageResponse;
        }
    }
}
