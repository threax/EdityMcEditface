using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public Action<SiteBuilderEventArgs> OnCustomizeSiteBuilder { get; set; }
    }
}
