using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer
{
    public class TemplateEnvironment
    {
        private Dictionary<String, String> vars = new Dictionary<string, string>();
        private HashSet<String> usedVars = new HashSet<string>();
        private LinkedContent linkedContent = new LinkedContent();
        private String docLink;
        private EdityProject project;
        private List<LinkedContentEntry> pageContent = null;
        private PageDefinition pageDefinition = new PageDefinition();

        public TemplateEnvironment(String docLink, EdityProject project)
        {
            this.docLink = docLink;
            this.project = project;
            linkedContent.mergeEntries(project.ContentMap);
        }

        public void buildVariables(PageDefinition page)
        {
            buildVariables(new PageDefinition[] { page });
        }

        public void buildVariables(IEnumerable<PageDefinition> pages)
        {
            usedVars.Clear();
            vars.Clear();
            foreach(var page in pages)
            {
                foreach(var var in page.Vars)
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
                vars.Add("editorRoot", "/");
            }
            if (!vars.ContainsKey("docLink"))
            {
                vars.Add("docLink", docLink);
            }

            List<LinkedContentEntry> links = new List<LinkedContentEntry>(linkedContent.buildResourceList(findLinkedContent(pages)));
            if (!vars.ContainsKey("css"))
            {
                vars.Add("css", linkedContent.renderCss(links));
            }
            if (!vars.ContainsKey("javascript"))
            {
                vars.Add("javascript", linkedContent.renderJavascript(links));
            }
        }

        private IEnumerable<String> findLinkedContent(IEnumerable<PageDefinition> pages)
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

        public String getVariable(String key, String defaultVal)
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
    }
}
