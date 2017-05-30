using System;
using System.Collections.Generic;
using System.Text;

namespace EdityMcEditface.Mvc.Models.Branch
{
    /// <summary>
    /// This interface will determine which branch the user is requesting.
    /// </summary>
    public interface IBranchDetector
    {
        /// <summary>
        /// Get the name of the branch the user requested.
        /// </summary>
        String RequestedBranch { get; }
    }
}
