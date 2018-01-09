using Halcyon.HAL.Attributes;
using Microsoft.AspNetCore.Http;

namespace EdityMcEditface.Mvc.Models.Page
{
    [HalModel]
    public class SavePageInput
    {
        public IFormFile Content { get; set; }

        //For some reason these input classes will not be written to the typescript client if they
        //only contain the content, this dummy property makes it work, this needs to be fixed in the future.
        public bool? DontSendThisNotUsed { get; set; }
    }
}
