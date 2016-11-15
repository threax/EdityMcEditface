using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.Models.Config
{
    /// <summary>
    /// The settings class.
    /// </summary>
    public class EditySettings
    {
        /// <summary>
        /// True to read only from the current directory.
        /// </summary>
        public bool ReadFromCurrentDirectory { get; set; }

        /// <summary>
        /// True to get detailed errors.
        /// </summary>
        public bool DetailedErrors { get; set; }

        /// <summary>
        /// True to enable no auth mode, this disables authentication completely.
        /// </summary>
        public bool NoAuth { get; set; } = false;

        /// <summary>
        /// The user account name to use when using NoAuth
        /// </summary>
        public String NoAuthUser { get; set; } = "OnlyUser";
    }
}
