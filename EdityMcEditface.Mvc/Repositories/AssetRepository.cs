using EdityMcEditface.HtmlRenderer;
using EdityMcEditface.Mvc.Models.Assets;
using EdityMcEditface.Mvc.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Threax.AspNetCore.FileRepository;

namespace EdityMcEditface.Mvc.Repositories
{
    public class AssetRepository : IAssetRepository
    {
        private IFileFinder fileFinder;
        private String pathBase;
        private IFileVerifier fileVerifier;

        public AssetRepository(IFileFinder fileFinder, IPathBaseInjector pathBaseInjector, IFileVerifier fileVerifier)
        {
            this.fileFinder = fileFinder;
            this.pathBase = pathBaseInjector.PathBase;
            this.fileVerifier = fileVerifier;
        }

        public async Task<ImageUploadResponse> AddAsset(ImageUploadInput arg)
        {
            ImageUploadResponse imageResponse = new ImageUploadResponse();

            try
            {
                using (var uploadStream = arg.Upload.OpenReadStream())
                {
                    fileVerifier.Validate(uploadStream, arg.Upload.FileName, arg.Upload.ContentType);
                    string autoFileFolder = "AutoUploads";
                    var autoFileFile = Guid.NewGuid().ToString() + Path.GetExtension(arg.Upload.FileName);
                    var autoPath = Path.Combine(autoFileFolder, autoFileFile);
                    using (Stream stream = fileFinder.WriteFile(autoPath))
                    {
                        await uploadStream.CopyToAsync(stream);
                    }

                    imageResponse.Uploaded = 1;
                    imageResponse.FileName = autoFileFile;
                    imageResponse.Url = pathBase + autoPath.EnsureStartingPathSlash();
                }
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
