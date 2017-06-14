using Halcyon.HAL.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace EdityMcEditface.Mvc.Models.Git
{
    [HalModel]
    public class MergeQuery
    {
        public String File { get; set; }
    }
}
