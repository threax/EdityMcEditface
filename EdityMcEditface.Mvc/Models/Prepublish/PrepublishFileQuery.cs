using Halcyon.HAL.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace EdityMcEditface.Mvc.Models.Prepublish
{
    [HalModel]
    public class PrepublishFileQuery
    {
        public String File { get; set; }
    }
}
