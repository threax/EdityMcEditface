﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer
{
    public class PageDefinition
    {
        /// <summary>
        /// The variables for this page.
        /// </summary>
        public Dictionary<String, String> Vars { get; set; } = new Dictionary<String, String>();

        /// <summary>
        /// The content linked to this page.
        /// </summary>
        public List<String> LinkedContent { get; set; } = new List<string>();

        /// <summary>
        /// True if the page should be hidden and not included in output.
        /// </summary>
        public bool Hidden { get; set; }

        /// <summary>
        /// Get the title of the page from the Vars, if the title does not exist, "Untitled" will be returned.
        /// </summary>
        [JsonIgnore]
        public String Title
        {
            get
            {
                String title;
                if(!Vars.TryGetValue("title", out title))
                {
                    title = "Untitled";
                }
                return title;
            }
        }

        public String Layout { get; set; } = "default.html";
    }
}
