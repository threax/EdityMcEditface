using System.Collections.Generic;

namespace EdityMcEditface.HtmlRenderer.Compiler
{
    public interface IContentCompilerFactory
    {
        ContentCompiler CreateCompiler(string inDir, string outDir, string backupPath, CompilerDefinition definition);
        List<ContentCompiler> CreateCompilers(string inDir, string outDir, string backupPath, IEnumerable<CompilerDefinition> definition);
    }
}