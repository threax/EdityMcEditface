using EdityMcEditface.HtmlRenderer.Compiler;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Threax.AspNetCore.BuiltInTools;
using Threax.AspNetCore.FileRepository;

namespace EdityMcEditface.Mvc.Config
{
    public class EdityEvents : IEdityEvents
    {
        public void CustomizeSiteBuilder(SiteBuilderEventArgs args)
        {
            if(OnCustomizeSiteBuilder != null)
            {
                OnCustomizeSiteBuilder.Invoke(args);
            }
        }

        /// <summary>
        /// Customize the site builder with additional options.
        /// </summary>
        public Action<SiteBuilderEventArgs> OnCustomizeSiteBuilder { get; set; }

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

        /// <summary>
        /// If you want to add custom named build tasks you can add them to the BuildTaskManager with this callback.
        /// </summary>
        public Action<BuildTaskManager> CustomizeBuildTasks { get; set; }
    }
}
