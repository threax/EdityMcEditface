using System;
using System.Collections.Generic;
using System.Text;

namespace EdityMcEditface.Mvc.Models.Phase
{
    /// <summary>
    /// This interface will determine which branch the user is requesting.
    /// </summary>
    public interface IPhaseDetector
    {
        /// <summary>
        /// Get or set the current phase.
        /// </summary>
        Phases Phase { get; set; }
    }
}
