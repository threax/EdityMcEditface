﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer
{
    public class TemplateEnvironment : ValueProvider
    {
        private Dictionary<String, String> vars = new Dictionary<string, string>();
        private HashSet<String> usedVars = new HashSet<string>();
        private LinkedContent linkedContent = new LinkedContent();
        private String docLink;
        private EdityProject project;
        private PageDefinition pageDefinition = new PageDefinition();
        private String pathBase;

        public TemplateEnvironment(String docLink, EdityProject project)
        {
            if(!project.Vars.TryGetValue("pathBase", out this.pathBase))
            {
                pathBase = "";
            }
            else
            {
                pathBase = pathBase.EnsureStartingPathSlash();
            }
            this.docLink = docLink.EnsureStartingPathSlash();
            this.project = project;
            linkedContent.mergeEntries(project.ContentMap);
        }

        /// <summary>
        /// Build the page variables
        /// </summary>
        /// <param name="pages">An enumerator over the pages in inside -> out order.</param>
        public void buildVariables(IEnumerable<PageStackItem> pages)
        {
            usedVars.Clear();
            vars.Clear();
            foreach(var page in pages)
            {
                foreach(var var in page.PageDefinition.Vars)
                {
                    mergeVar(var);
                }
            }
            foreach (var var in project.Vars)
            {
                mergeVar(var);
            }
            
            if (!vars.ContainsKey("editorRoot"))
            {
                vars.Add("editorRoot", pathBase);
            }
            vars["docLink"] = docLink;
            vars["pathBase"] = pathBase; //This ensure we use what was in the edity settings, so pages can't overwrite this.

            List<LinkedContentEntry> links = new List<LinkedContentEntry>(linkedContent.buildResourceList(findLinkedContent(pages.Select(p => p.PageDefinition))));
            vars["css"] = linkedContent.renderCss(links, pages.Where(p => p.PageCssPath != null).Select(p => "~" + p.PageCssPath));
            vars["javascript"] = linkedContent.renderJavascript(links, pages.Where(p => p.PageScriptPath != null).Select(p => new JavascriptEntry() { File = "~" + p.PageScriptPath }));
        }

        public IEnumerable<String> findLinkedContent(IEnumerable<PageDefinition> pages)
        {
            foreach(var page in pages)
            {
                foreach(var content in page.LinkedContent)
                {
                    yield return content;
                }
            }
            foreach(var content in project.LinkedContent)
            {
                yield return content;
            }
        }

        private void mergeVar(KeyValuePair<string, string> var)
        {
            if (!vars.ContainsKey(var.Key))
            {
                vars.Add(var.Key, var.Value);
            }
        }

        public String getValue(String key, String defaultVal)
        {
            usedVars.Add(key);
            String value;
            if(vars.TryGetValue(key, out value))
            {
                return value;
            }
            return defaultVal;
        }

        /// <summary>
        /// This will be true unless key is css or javascript, these are the only variables allowed 
        /// to write raw. Be sure to encode everything else.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool shouldEncodeOutput(String key)
        {
            //ONLY the css and javascript tags should be written insecurly
            return key != "css" && key != "javascript";
        }

        /// <summary>
        /// The variables in the collection
        /// </summary>
        public IEnumerable<KeyValuePair<String, String>> Variables
        {
            get
            {
                return vars;
            }
        }

        /// <summary>
        /// A collection of vars that have been used already
        /// </summary>
        public IEnumerable<String> UsedVars
        {
            get
            {
                return usedVars;
            }
        }

        public LinkedContent LinkedContent
        {
            get
            {
                return linkedContent;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public String PathBase
        {
            get
            {
                return this.pathBase;
            }
        }
    }
}
