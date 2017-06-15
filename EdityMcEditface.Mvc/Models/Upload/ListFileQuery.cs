using Halcyon.HAL.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace EdityMcEditface.Mvc.Models.Upload
{
    [HalModel]
    public class ListFileQuery
    {
        public String Dir { get; set; }
    }
}
