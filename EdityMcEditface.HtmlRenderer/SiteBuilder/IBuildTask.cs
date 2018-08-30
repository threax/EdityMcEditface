using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer.SiteBuilder
{
    public interface IBuildTask
    {
        Task Execute(BuildEventArgs args);
    }
}
