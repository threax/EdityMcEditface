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
        private IFileFinder fileFinder;

        public PageStack(TemplateEnvironment environment, IFileFinder fileFinder)
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

        /// <summary>
        /// The file with the content.
        /// </summary>
        public String ContentFile { get; set; }

        /// <summary>
        /// Set this func to a function that takes a string and returns a string to transform
        /// the content file before it is used.
        /// </summary>
        public Func<String, String> ContentTransformer { get; set; }

        private IEnumerable<PageStackItem> pages;
        public IEnumerable<PageStackItem> Pages
        {
            get
            {
                //Caching the pages avoids a huge number of filesystem reads
                if(pages == null)
                {
                    pages = LoadPageStack().ToList();
                }
                return pages;
            }
        }

        private IEnumerable<PageStackItem> LoadPageStack()
        {
            if (ContentFile != null)
            {
                var content = fileFinder.LoadPageStackContent(ContentFile);
                if (ContentTransformer != null)
                {
                    content.Content = ContentTransformer(content.Content);
                }
                yield return content;
            }
            for (int i = layouts.Count - 1; i >= 0; --i)
            {
                yield return fileFinder.LoadPageStackLayout(layouts[i]);
            }
        }

        /// <summary>
        /// Determine if the ContentFile is allowed to be rendered. if no content file is set this will be false.
        /// </summary>
        public bool Visible
        {
            get
            {
                if (ContentFile != null)
                {
                    var content = fileFinder.LoadPageStackContent(ContentFile);
                    if (ContentTransformer != null)
                    {
                        content.Content = ContentTransformer(content.Content);
                    }
                    return !content.PageDefinition.Hidden;
                }
                return false;
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
                    foreach (var js in link.Js)
                    {
                        yield return js.File;
                    }
                }
            }
        }
    }
}
