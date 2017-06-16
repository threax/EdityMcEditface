using Halcyon.HAL.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using Threax.AspNetCore.Halcyon.Ext;
using Threax.AspNetCore.Halcyon.Ext.UIAttrs;

namespace EdityMcEditface.Mvc.Models
{
    [HalModel]
    public class DraftQuery : PagedCollectionQuery
    {
        /// <summary>
        /// Set this to a specific file to load only the draft info for that file.
        /// </summary>
        public String File { get; set; }

        /// <summary>
        /// Set this to true to show only changed files.
        /// </summary>
        [UiSearch]
        [Display(Name = "Show Only Changed Files")]
        public bool ShowChangedOnly { get; set; }
    }
}
