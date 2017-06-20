using System.Threading.Tasks;
using EdityMcEditface.Mvc.Models.Compiler;
using EdityMcEditface.Mvc.Models.Page;

namespace EdityMcEditface.Mvc.Repositories
{
    public interface IPublishRepository
    {
        void Compile();

        Task<PublishEntryPoint> GetPublishInfo(ISyncRepository syncRepo);

        CompileProgress Progress();
    }
}