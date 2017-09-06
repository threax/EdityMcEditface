using System;
using System.Collections.Generic;
using System.Text;

namespace EdityMcEditface.Mvc.Services
{
    public interface IOverrideValuesProvider
    {
        /// <summary>
        /// Get the override variables set for the project, can be null, which means no overrides.
        /// </summary>
        Dictionary<String, String> OverrideVars { get; }
    }
}
