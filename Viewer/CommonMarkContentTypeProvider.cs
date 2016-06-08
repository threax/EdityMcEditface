using Microsoft.Owin.StaticFiles.ContentTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viewer
{
    class CommonMarkContentTypeProvider : FileExtensionContentTypeProvider
    {
        public CommonMarkContentTypeProvider()
        {
            Mappings.Add(".json", "application/json");
            Mappings.Remove(".html");
        }
    }
}
