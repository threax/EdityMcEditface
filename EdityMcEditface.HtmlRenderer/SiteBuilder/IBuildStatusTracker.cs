using System.Collections.Generic;

namespace EdityMcEditface.HtmlRenderer.SiteBuilder
{
    public interface IBuildStatusTracker
    {
        void AddMessage(string message);
        List<string> GetMessages();
    }
}