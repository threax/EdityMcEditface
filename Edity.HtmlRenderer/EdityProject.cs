using EdityMcEditface.HtmlRenderer.Compiler;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer
{
    public class EdityProject
    {
        public Dictionary<String, LinkedContentEntry> ContentMap { get; set; } = new Dictionary<string, LinkedContentEntry>();

        public Dictionary<String, String> Vars { get; set; } = new Dictionary<string, string>();

        public List<String> LinkedContent { get; set; } = new List<string>();

        public List<String> AdditionalContent { get; set; } = new List<string>();

        public List<CompilerDefinition> Compilers { get; set; } = new List<CompilerDefinition>();

        public List<String> EditComponents { get; set; } = new List<String>();

        /// <summary>
        /// Merge two projects together. Does not fully copy the other project and will use objects
        /// from inside it, don't keep making changes to the project merged into this one or the
        /// changes will be in both projects.
        /// </summary>
        /// <param name="backupProject"></param>
        public void merge(EdityProject backupProject)
        {
            mergeDictionarys(backupProject.ContentMap, ContentMap);
            mergeDictionarys(backupProject.Vars, Vars);
            mergeStringList(backupProject.LinkedContent, LinkedContent);
            mergeStringList(backupProject.AdditionalContent, AdditionalContent);
            mergeStringList(backupProject.EditComponents, EditComponents);
            mergeCompilerLists(backupProject.Compilers, Compilers);

        }

        private static void mergeDictionarys<TKey, TValue>(Dictionary<TKey, TValue> source, Dictionary<TKey, TValue> dest)
        {
            foreach (var item in source)
            {
                if (!dest.ContainsKey(item.Key))
                {
                    dest.Add(item.Key, item.Value);
                }
            }
        }

        private static void mergeStringList(List<String> source, List<String> dest)
        {
            foreach(var item in source)
            {
                if (!dest.Contains(item))
                {
                    dest.Add(item);
                }
            }
        }

        private static void mergeCompilerLists(List<CompilerDefinition> source, List<CompilerDefinition> dest)
        {
            foreach (var item in source)
            {
                if (!dest.Select(i => i.Extension == item.Extension).Any())
                {
                    dest.Add(item);
                }
            }
        }
    }
}