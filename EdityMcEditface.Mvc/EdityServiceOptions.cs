using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Threax.AspNetCore.BuiltInTools;
using Threax.AspNetCore.FileRepository;

namespace EdityMcEditface.Mvc
{
    /// <summary>
    /// Additional options for setting up edity services.
    /// </summary>
    public class EdityServiceOptions
    {
        /// <summary>
        /// If you need to do additional customizations use this action. The halcyon configuration
        /// and edity controllers will already be configured.
        /// </summary>
        public Action<IMvcBuilder> CustomizeMvcBuilder { get; set; }

        /// <summary>
        /// If you need to add additional file type support use this action.
        /// </summary>
        public Action<IFileVerifier> CustomizeFileVerifier { get; set; }

        /// <summary>
        /// Customize the tool runner. It will already be configured with the client generator tools.
        /// </summary>
        public Action<IToolRunner> CustomizeTools { get; set; }
    }
}
