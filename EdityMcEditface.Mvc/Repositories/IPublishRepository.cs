using System.Threading.Tasks;
using EdityMcEditface.Mvc.Models.Compiler;
using EdityMcEditface.Mvc.Models.Page;

namespace EdityMcEditface.Mvc.Repositories
{
    public interface IPublishRepository
    {
        Task<CompileResult> Compile();

        Task<PublishEntryPoint> Status(ISyncRepository syncRepo);
    }
}