using System;
using System.Collections.Generic;

namespace EdityMcEditface.HtmlRenderer.Compiler
{
    public interface IContentCompilerFactory
    {
        List<IContentCompiler> CreateCompilers(IFileFinder fileFinder, String outDir, IEnumerable<CompilerDefinition> definition);
        IContentCompiler CreateCompiler(IFileFinder fileFinder, String outDir, CompilerDefinition definition);
    }
}