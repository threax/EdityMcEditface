using EdityMcEditface.HtmlRenderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SiteCompiler
{
    public static class ContentCompilerFactory
    {
        public static List<ContentCompiler> CreateCompilers(String inDir, String outDir, String backupPath, IEnumerable<CompilerDefinition> definition)
        {
            return new List<ContentCompiler>(definition.Select(d => CreateCompiler(inDir, outDir, backupPath, d)));
        }

        public static ContentCompiler CreateCompiler(String inDir, String outDir, String backupPath, CompilerDefinition definition)
        {
            switch (definition.Type)
            {
                case CompilerTypes.Html:
                    return new HtmlCompiler(inDir, outDir, backupPath, definition.Template)
                    {
                        OutputExtension = definition.Extension
                    };
                default:
                    throw new NotSupportedException($"Compiler type {definition.Type} not supported.");
            }
        }
    }
}
