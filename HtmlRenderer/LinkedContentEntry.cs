﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer
{
    public class LinkedContentEntry
    {
        [JsonProperty]
        private List<String> css = new List<string>();
        [JsonProperty]
        private List<String> javascript = new List<string>();
        private bool hasMin;

        public LinkedContentEntry(bool hasMin = true)
        {
            this.hasMin = hasMin;
        }

        public void addCss(String file)
        {
            css.Add(file);
        }

        public void addJavascript(String file)
        {
            javascript.Add(file);
        }

        public bool HasMin { get; set; }

        public IEnumerable<String> Css
        {
            get
            {
                return css;
            }
        }

        public IEnumerable<String> Javascript
        {
            get
            {
                return javascript;
            }
        }
    }
}
