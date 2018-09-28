using System;
using System.Collections.Generic;
using System.Text;

namespace EdityMcEditface.HtmlRenderer
{
    public class DefaultOverrideValuesProvider : IOverrideValuesProvider
    {
        public DefaultOverrideValuesProvider(Dictionary<String, String> overrideVars)
        {
            this.OverrideVars = overrideVars;
        }

        public Dictionary<string, string> OverrideVars { get; private set; }
    }
}
