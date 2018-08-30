using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer.SiteBuilder
{
    public interface IPublishTask
    {
        Task Execute(BuildEventArgs args);
    }
}
