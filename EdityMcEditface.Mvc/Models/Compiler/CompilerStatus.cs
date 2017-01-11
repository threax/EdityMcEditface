using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.Mvc.Models.Compiler
{
    public class CompilerStatus
    {
        public int BehindBy { get; internal set; }
        public object BehindHistory { get; internal set; }
    }
}
