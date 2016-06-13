﻿using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdityMcEditface
{
    class CommonMarkContentTypeProvider : FileExtensionContentTypeProvider
    {
        public CommonMarkContentTypeProvider()
        {
            Mappings.Remove(".html");
        }
    }
}