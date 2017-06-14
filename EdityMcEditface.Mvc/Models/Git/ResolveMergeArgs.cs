using Halcyon.HAL.Attributes;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace EdityMcEditface.Mvc.Models.Git
{
    [HalModel]
    public class ResolveMergeArgs
    {
        public IFormFile Content { get; set; }
    }
}
