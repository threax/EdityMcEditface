using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SiteCompiler
{
    public interface ContentCompiler
    {
        void buildPage(string relativeFile);
        void copyProjectContent();
    }
}
