using System.Threading.Tasks;
using EdityMcEditface.Mvc.Models.Page;
using Microsoft.AspNetCore.Http;
using System;
using EdityMcEditface.Mvc.Models.Assets;

namespace EdityMcEditface.Mvc.Repositories
{
    public interface IPageRepository
    {
        void Delete(string page);
        PageSettings GetSettings(string page);
        Task Save(string page, IFormFile content);
        void UpdateSettings(string page, PageSettings settings);
        Task<PageInfoCollection> List(PageQuery query);
    }
}