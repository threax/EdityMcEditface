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

        public TemplateEnvironment(String docLink, EdityProject project)
        {
            vars.Add("editorRoot", "/");
            vars.Add("docLink", docLink);
        }

        public void setVariable(String key, String value)
        {
            vars[key] = value;
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
