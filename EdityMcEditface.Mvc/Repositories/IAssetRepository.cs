using System.Threading.Tasks;
using EdityMcEditface.Mvc.Models.Assets;

namespace EdityMcEditface.Mvc.Repositories
{
    public interface IAssetRepository
    {
        Task<ImageUploadResponse> AddAsset(ImageUploadInput arg);
    }
}