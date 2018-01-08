using System;
using System.Collections.Generic;
using System.Text;

namespace EdityMcEditface.Mvc.Models.Phase
{
    /// <summary>
    /// The phases for a document.
    /// </summary>
    public enum Phases
    {
        /// <summary>
        /// The edit phase is where all collaborators create and share their changes.
        /// It maps to the latest files in the git repo.
        /// </summary>
        Edit,

        /// <summary>
        /// The draft phase is the version of the document that will be published when
        /// the site is published.
        /// </summary>
        Draft
    }
}
