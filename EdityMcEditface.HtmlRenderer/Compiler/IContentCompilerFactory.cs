using System.Collections.Generic;

namespace EdityMcEditface.HtmlRenderer.Compiler
{
    public interface IContentCompilerFactory
    {
        IContentCompiler CreateCompiler(string inDir, string outDir, string backupPath, CompilerDefinition definition);
        List<IContentCompiler> CreateCompilers(string inDir, string outDir, string backupPath, IEnumerable<CompilerDefinition> definition);
    }
}