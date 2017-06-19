using EdityMcEditface.Mvc.Controllers;
using Halcyon.HAL.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Threax.AspNetCore.Halcyon.Ext;

namespace EdityMcEditface.Mvc.Models.Assets
{
    [HalModel]
    [DeclareHalLink(EntryPointController.Rels.Get)]
    public class ImageUploadResponse
    {
        /// <summary>
        /// Set to 1 for uploaded or 0 for not uploaded
        /// </summary>
        public int Uploaded { get; set; }

        /// <summary>
        /// The name of the saved file
        /// </summary>
        public String FileName { get; set; }

        /// <summary>
        /// The url to the saved file
        /// </summary>
        public String Url { get; set; }

        /// <summary>
        /// A message to display
        /// </summary>
        public String Message { get; set; }
    }
}
