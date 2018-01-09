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

        //For some reason these input classes will not be written to the typescript client if they
        //only contain the content, this dummy property makes it work, this needs to be fixed in the future.
        public bool? DontSendThisNotUsed { get; set; }
    }
}
