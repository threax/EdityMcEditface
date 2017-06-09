using System.Threading.Tasks;
using EdityMcEditface.Mvc.Models.Git;
using LibGit2Sharp;

namespace EdityMcEditface.Mvc.Repositories
{
    public interface ISyncRepository
    {
        Task Pull(Signature signature);
        Task Push();
        Task<SyncInfo> SyncInfo();
    }
}