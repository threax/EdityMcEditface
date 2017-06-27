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
        /// <summary>
        /// This map maps named content to a collection of javascript and css files.
        /// </summary>
        public Dictionary<String, LinkedContentEntry> ContentMap { get; set; } = new Dictionary<string, LinkedContentEntry>();

        /// <summary>
        /// The main set of variables, this is always active.
        /// </summary>
        public Dictionary<String, String> Vars { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Variables that are used only during build / compile / publish. These can override the values in Vars.
        /// </summary>
        public Dictionary<String, String> BuildVars { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// The list of linked content for the entire project.
        /// </summary>
        public List<String> LinkedContent { get; set; } = new List<string>();

        /// <summary>
        /// Additional content folders to copy.
        /// </summary>
        public List<String> AdditionalContent { get; set; } = new List<string>();

        /// <summary>
        /// Definitions of the compilers.
        /// </summary>
        public List<CompilerDefinition> Compilers { get; set; } = new List<CompilerDefinition>();

        /// <summary>
        /// The list of components to show when editing.
        /// </summary>
        public List<String> EditComponents { get; set; } = new List<String>();

        /// <summary>
        /// The list of components to show in draft mode.
        /// </summary>
        public List<String> DraftComponents { get; set; } = new List<String>();

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
            mergeDictionarys(backupProject.BuildVars, BuildVars);
            mergeStringList(backupProject.LinkedContent, LinkedContent);
            mergeStringList(backupProject.AdditionalContent, AdditionalContent);
            mergeStringList(backupProject.EditComponents, EditComponents);
            mergeStringList(backupProject.DraftComponents, DraftComponents);
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
                if (!dest.Any(i => i.Extension == item.Extension))
                {
                    dest.Add(item);
                }
            }
        }
    }
}