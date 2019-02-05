using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer
{
    public class PageStackItem
    {
        private Func<String> getContentCb;

        public PageStackItem(Func<String> getContentCb)
        {
            this.getContentCb = getContentCb;
        }

        public PageDefinition PageDefinition { get; set; }

        private string content;
        public String Content
        {
            get
            {
                if(content == null)
                {
                    content = getContentCb();
                }
                return content;
            }
            set
            {
                this.content = value;
            }
        }

        public String PageScriptPath { get; set; }

        public String PageCssPath { get; set; }
    }
}
