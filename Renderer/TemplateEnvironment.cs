using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkTools.Renderer
{
    public class TemplateEnvironment
    {
        private Dictionary<String, String> vars = new Dictionary<string, string>();

        public TemplateEnvironment(String docLink)
        {
            vars.Add("EditorRoot", "/");
            vars.Add("DocLink", docLink);
        }

        public String getVariable(String key, String defaultVal)
        {
            String value;
            if(vars.TryGetValue(key, out value))
            {
                return value;
            }
            return defaultVal;
        }

        public IEnumerable<KeyValuePair<String, String>> Variables
        {
            get
            {
                return vars;
            }
        }
    }
}
