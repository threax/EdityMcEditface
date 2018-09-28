using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer
{
    public class TemplateEnvironment : ValueProvider
    {
        private Dictionary<String, String> vars = new Dictionary<string, string>();
        private Dictionary<String, String> overrideVars;
        private LinkedContent linkedContent = new LinkedContent();
        private String docLink;
        private EdityProject project;
        private PageDefinition pageDefinition = new PageDefinition();
        private String pathBase;
        private IFileFinder fileFinder;
        private String version; //The version of the website, helps generate query strings for files to get around cache issues
        private bool useBuildVars;

        public TemplateEnvironment(String docLink, IFileFinder fileFinder, Dictionary<String, String> overrideVars, String version = null, bool useBuildVars = false)
        {
            this.overrideVars = overrideVars;
            this.useBuildVars = useBuildVars;
            this.project = fileFinder.Project;
            this.version = version;

            if (!(useBuildVars && project.BuildVars.TryGetValue("pathBase", out this.pathBase)) && !project.Vars.TryGetValue("pathBase", out this.pathBase))
            {
                pathBase = "";
            }
            else
            {
                pathBase = pathBase.EnsureStartingPathSlash();
            }

            this.project.EditPageSettings["baseUrl"] = pathBase;

            this.docLink = docLink.EnsureStartingPathSlash();
            this.fileFinder = fileFinder;
            linkedContent.mergeEntries(project.ContentMap);
        }

        /// <summary>
        /// Build the page variables
        /// </summary>
        /// <param name="pages">An enumerator over the pages in inside -> out order.</param>
        public void buildVariables(IEnumerable<PageStackItem> pages)
        {
            vars.Clear();
            //Bring in the override vars that were specified, if they were given
            if(overrideVars != null)
            {
                foreach(var var in overrideVars)
                {
                    mergeVar(var);
                }
            }

            //Bring in page vars, these will override build and project vars
            foreach(var page in pages)
            {
                foreach(var var in page.PageDefinition.Vars)
                {
                    mergeVar(var);
                }
            }

            //If active merge build vars first so they are the active variables
            if (useBuildVars)
            {
                foreach (var var in project.BuildVars)
                {
                    mergeVar(var);
                }
            }

            //Then merge in the project vars
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
            vars["css"] = linkedContent.renderCss(links, pages.Where(p => p.PageCssPath != null).Select(p => "~" + p.PageCssPath), version);
            vars["javascript"] = linkedContent.renderJavascript(links, pages.Where(p => p.PageScriptPath != null).Select(p => new JavascriptEntry() { File = "~" + p.PageScriptPath }), version);
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

        private const String sectionOpen = "section(";
        private const String editPageSettings = "editPageSettings()";
        private const String macroClose = ")";

        public String getValue(String key, String defaultVal)
        {
            if(key.StartsWith(sectionOpen) && key.EndsWith(macroClose))
            {
                var sectionName = key.Substring(sectionOpen.Length, key.Length - sectionOpen.Length - macroClose.Length);
                try
                {
                    return fileFinder.LoadSection(sectionName);
                }
                catch (IOException ex)
                {
                    //Return an error message
                    return $"{ex.GetType().Name} Message: {ex.Message}";
                }
            }
            else if (key.Equals(editPageSettings))
            {
                return 
$@"<script type=""text/javascript"">
window.hr_config = (function(next){{
    return function(config)
    {{
        config.editSettings = {JsonWriter.Serialize(this.project.EditPageSettings)};
        return next ? next(config) : config;
    }}
}})(window.hr_config);
</script>";
            }
            else
            {
                String value;
                if (vars.TryGetValue(key, out value))
                {
                    //Expand any paths that start with ~/, can be escaped with \~/
                    if(value.StartsWith("~/"))
                    {
                        value = pathBase + value.Substring(1);
                    }
                    else if(value.StartsWith("\\~/"))
                    {
                        value = value.Substring(1);
                    }
                    return value;
                }
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
            //The css and javascript tags should be written insecurly, also the section macro
            return key != "css" && key != "javascript" && !key.StartsWith(sectionOpen) && !key.Equals(editPageSettings);
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
