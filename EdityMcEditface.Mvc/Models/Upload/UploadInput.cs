using Halcyon.HAL.Attributes;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace EdityMcEditface.Mvc.Models.Upload
{
    [HalModel]
    public class UploadInput
    {
        public String File { get; set; }

        public IFormFile Content { get; set; }
    }
}
