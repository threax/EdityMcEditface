using Halcyon.HAL.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.Mvc.Models.Git
{
    [HalModel]
    public class NewCommit
    {
        public String Message { get; set; }
    }
}
