﻿using EdityMcEditface.HtmlRenderer.Compiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer.SiteBuilder
{
    public class SiteBuilderSettings
    {
        public String OutDir { get; set; }

        public String EdityDir { get; set; } = "edity";

        public string SiteName { get; set; }

        private IContentCompilerFactory CompilerFactory { get; set; } = new ContentCompilerFactory();
    }
}
