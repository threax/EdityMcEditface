using System;
using System.Collections.Generic;
using System.Text;

namespace EdityMcEditface.Mvc.Models.Compiler
{
    public interface ICompileRequestDetector
    {
        bool IsCompileRequest { get; }
    }
}
