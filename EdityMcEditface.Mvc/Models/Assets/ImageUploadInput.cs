using Halcyon.HAL.Attributes;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace EdityMcEditface.Mvc.Models.Assets
{
    [HalModel]
    public class ImageUploadInput
    {
        public IFormFile Upload { get; set; }
    }
}
