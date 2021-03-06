﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer.Compiler
{
    public enum CompilerTypes
    {
        Html,
        Json,
    }

    public class CompilerDefinition
    {
        public CompilerTypes Type { get; set; } = CompilerTypes.Html;

        public String Extension { get; set; } = null;
    }
}
