using EdityMcEditface.Mvc.Models.Git;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace EdityMcEditface.Mvc.Repositories
{
    public interface IMergeRepository
    {
        MergeInfo MergeInfo(string file);

        Task Resolve(String file, IFormFile content);
    }
}