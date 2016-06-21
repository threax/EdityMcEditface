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

        public void merge(EdityProject backupProject)
        {
            mergeDictionarys(backupProject.ContentMap, ContentMap);
            mergeDictionarys(backupProject.Vars, Vars);
            mergeList(backupProject.LinkedContent, LinkedContent);
            mergeList(backupProject.AdditionalContent, AdditionalContent);
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

        private static void mergeList<T>(List<T> source, List<T> dest)
        {
            foreach(var item in source)
            {
                if (!dest.Contains(item))
                {
                    dest.Add(item);
                }
            }
        }
    }
}