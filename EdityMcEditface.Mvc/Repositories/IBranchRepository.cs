using EdityMcEditface.Mvc.Models.Branch;
using LibGit2Sharp;

namespace EdityMcEditface.Mvc.Repositories
{
    public interface IBranchRepository
    {
        void Add(string name);
        void Checkout(string name, Signature sig);
        Models.Branch.BranchCollection List();
        BranchView GetCurrent();
    }
}