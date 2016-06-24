using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer
{
    public class PageStack
    {
        private TemplateEnvironment environment;
        private List<String> layouts = new List<string>();
        private FileFinder fileFinder;

        public PageStack(TemplateEnvironment environment, FileFinder fileFinder)
        {
            this.environment = environment;
            this.fileFinder = fileFinder;
        }

        /// <summary>
        /// Add a layout to the finder. This should be just the file name of the layout, including extension.
        /// </summary>
        /// <param name="layout"></param>
        public void pushLayout(String layout)
        {
            this.layouts.Add(layout);
        }

        /// <summary>
        /// Clear the layouts
        /// </summary>
        public void clearLayout()
        {
            this.layouts.Clear();
        }

        public TemplateEnvironment Environment
        {
            get
            {
                return environment;
            }
        }

        public String ContentFile { get; set; }

        public IEnumerable<PageStackItem> Pages
        {
            get
            {
                if (ContentFile != null)
                {
                    yield return fileFinder.loadPageStackContent(ContentFile);
                }
                for (int i = layouts.Count - 1; i >= 0; --i)
                {
                    yield return fileFinder.loadPageStackLayout(layouts[i]);
                }
            }
        }

        public IEnumerable<LinkedContent> LinkedContent { get; set; }

        public IEnumerable<String> LinkedContentFiles
        {
            get
            {
                List<LinkedContentEntry> links = new List<LinkedContentEntry>(environment.LinkedContent.buildResourceList(environment.findLinkedContent(Pages.Select(p => p.PageDefinition))));
                foreach (var link in links)
                {
                    foreach (var css in link.Css)
                    {
                        yield return css;
                    }
                    foreach (var js in link.Javascript)
                    {
                        yield return js.File;
                    }
                }
            }
        }
    }
}
