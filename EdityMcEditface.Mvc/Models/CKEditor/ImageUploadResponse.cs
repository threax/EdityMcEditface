using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.Mvc.Models.CKEditor
{
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
