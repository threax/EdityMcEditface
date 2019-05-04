using System.Threading.Tasks;
using EdityMcEditface.Mvc.Auth;
using EdityMcEditface.Mvc.Models.Compiler;
using EdityMcEditface.Mvc.Models.Page;

namespace EdityMcEditface.Mvc.Repositories
{
    public interface IPublishRepository
    {
        void Compile(IUserInfo compilingUser);

        Task<PublishEntryPoint> GetPublishInfo(ISyncRepository syncRepo);

        CompileProgress Progress();
    }
}