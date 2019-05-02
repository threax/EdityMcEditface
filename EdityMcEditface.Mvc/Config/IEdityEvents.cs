using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Threax.AspNetCore.BuiltInTools;
using Threax.AspNetCore.FileRepository;

namespace EdityMcEditface.Mvc.Config
{
    public interface IEdityEvents
    {
        /// <summary>
        /// Called when a sitebuilder is created, extra events can be added to it here.
        /// </summary>
        /// <param name="args">The args for the event.</param>
        void CustomizeSiteBuilder(SiteBuilderEventArgs args);

        /// <summary>
        /// If you need to do additional customizations use this action. The halcyon configuration
        /// and edity controllers will already be configured.
        /// </summary>
        Action<IMvcBuilder> CustomizeMvcBuilder { get; set; }

        /// <summary>
        /// If you need to add additional file type support use this action.
        /// </summary>
        Action<IFileVerifier> CustomizeFileVerifier { get; set; }

        /// <summary>
        /// Customize the tool runner. It will already be configured with the client generator tools.
        /// </summary>
        Action<IToolRunner> CustomizeTools { get; set; }
    }
}
