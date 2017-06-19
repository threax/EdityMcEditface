using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer
{
    public class LinkedContent
    {
        private const String VersionQuery = "ver";

        private Dictionary<String, LinkedContentEntry> entries = new Dictionary<String, LinkedContentEntry>();

        public LinkedContent()
        {

        }

        /// <summary>
        /// Add everything from other that is not already in this collection
        /// </summary>
        /// <param name="other"></param>
        public void mergeEntries(IEnumerable<KeyValuePair<String, LinkedContentEntry>> other)
        {
            foreach (var entry in other)
            {
                addEntry(entry.Key, entry.Value);
            }
        }

        public void addEntry(String key, LinkedContentEntry entry)
        {
            if (!entries.ContainsKey(key))
            {
                entries.Add(key, entry);
            }
        }

        public IEnumerable<LinkedContentEntry> buildResourceList(IEnumerable<String> resources)
        {
            HashSet<LinkedContentEntry> usedItems = new HashSet<LinkedContentEntry>();
            Stack<LinkedContentEntry> dependencyStack = new Stack<LinkedContentEntry>();
            foreach (var key in resources.Reverse())
            {
                buildDependencyStack(dependencyStack, key);
            }
            while (dependencyStack.Count > 0)
            {
                var current = dependencyStack.Pop();
                if (!usedItems.Contains(current))
                {
                    usedItems.Add(current);
                    yield return current;
                }
            }
        }

        private void buildDependencyStack(Stack<LinkedContentEntry> dependencyStack, String currentItem)
        {
            //Yep recursion and a huge stack, you also have to feed in the list backwards, go computers
            LinkedContentEntry entry;
            if (entries.TryGetValue(currentItem, out entry))
            {
                dependencyStack.Push(entry);
                foreach (var dep in entry.Dependencies)
                {
                    buildDependencyStack(dependencyStack, dep);
                }
            }
        }

        private const String cssHtml = "<link rel=\"stylesheet\" href=\"{0}\" type=\"text/css\" />";

        public String renderCss(IEnumerable<LinkedContentEntry> entries, IEnumerable<String> additionalFiles, String version)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var entry in entries)
            {
                foreach (var css in entry.Css)
                {
                    sb.AppendFormat(cssHtml, GetVersionedPath(version, css));
                    sb.AppendLine();
                }
            }
            foreach(var file in additionalFiles)
            {
                sb.AppendFormat(cssHtml, GetVersionedPath(version, file));
                sb.AppendLine();
            }
            return sb.ToString();
        }

        private const String javascriptHtml = "<script type=\"text/javascript\" src=\"{0}\"></script>";
        private const String javascriptAsyncHtml = "<script type=\"text/javascript\" src=\"{0}\" async></script>";

        public String renderJavascript(IEnumerable<LinkedContentEntry> entries, IEnumerable<JavascriptEntry> additionalFiles, String version)
        {
            StringBuilder sb = new StringBuilder();
            String currentFormat;
            foreach (var entry in entries)
            {
                foreach (var js in entry.Js)
                {
                    currentFormat = getTag(js);
                    sb.AppendFormat(currentFormat, GetVersionedPath(version, js.File));
                    sb.AppendLine();
                }
            }
            foreach(var js in additionalFiles)
            {
                currentFormat = getTag(js);
                sb.AppendFormat(currentFormat, GetVersionedPath(version, js.File));
                sb.AppendLine();
            }
            return sb.ToString();
        }

        private static String GetVersionedPath(string version, string filePath)
        {
            var realFilePath = filePath;
            if (!String.IsNullOrEmpty(version))
            {
                var queryIndex = filePath.IndexOf('?');
                if (queryIndex != -1)
                {
                    realFilePath = filePath.Insert(queryIndex + 1, $"{VersionQuery}={version}&");
                }
                else
                {
                    realFilePath = filePath + $"?{VersionQuery}={version}";
                }
            }
            return realFilePath;
        }

        private static string getTag(JavascriptEntry js)
        {
            string currentFormat;
            if (js.Async)
            {
                currentFormat = javascriptAsyncHtml;
            }
            else
            {
                currentFormat = javascriptHtml;
            }

            return currentFormat;
        }
    }
}
