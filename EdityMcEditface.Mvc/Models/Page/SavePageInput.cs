using Halcyon.HAL.Attributes;
using Microsoft.AspNetCore.Http;

namespace EdityMcEditface.Mvc.Models.Page
{
    [HalModel]
    public class SavePageInput
    {
        public IFormFile Content { get; set; }
    }
}
