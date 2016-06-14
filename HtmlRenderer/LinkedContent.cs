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

        public String renderCss(IEnumerable<LinkedContentEntry> entries)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var entry in entries)
            {
                foreach (var css in entry.Css)
                {
                    sb.AppendLine($@"<link rel=""stylesheet"" href=""{css}"" type=""text/css"" />");
                }
            }
            return sb.ToString();
        }

        public String renderJavascript(IEnumerable<LinkedContentEntry> entries)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var entry in entries)
            {
                foreach (var js in entry.Javascript)
                {
                    sb.AppendLine($@"<script type=""text/javascript"" src=""{js}""></script>");
                }
            }
            return sb.ToString();
        }
    }
}
