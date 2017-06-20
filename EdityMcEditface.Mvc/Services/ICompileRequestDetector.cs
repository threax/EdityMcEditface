using System;
using System.Collections.Generic;
using System.Text;

namespace EdityMcEditface.Mvc.Services
{
    public interface ICompileRequestDetector
    {
        bool IsCompileRequest { get; }
    }
}
