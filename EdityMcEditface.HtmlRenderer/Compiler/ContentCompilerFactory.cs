using EdityMcEditface.HtmlRenderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer.Compiler
{
    public class ContentCompilerFactory : IContentCompilerFactory
    {
        public ContentCompilerFactory()
        {

        }

        public List<ContentCompiler> CreateCompilers(String inDir, String outDir, String backupPath, IEnumerable<CompilerDefinition> definition)
        {
            return new List<ContentCompiler>(definition.Select(d => CreateCompiler(inDir, outDir, backupPath, d)));
        }

        public ContentCompiler CreateCompiler(String inDir, String outDir, String backupPath, CompilerDefinition definition)
        {
            switch (definition.Type)
            {
                case CompilerTypes.Html:
                    return new HtmlCompiler(inDir, outDir, backupPath, definition.Template, definition.Settings)
                    {
                        OutputExtension = definition.Extension
                    };
                case CompilerTypes.Pdf:
                    return new PdfCompiler(inDir, outDir, backupPath, definition.Template);
                case CompilerTypes.Json:
                    return new JsonCompiler(inDir, outDir, backupPath, definition.Template)
                    {
                        OutputExtension = definition.Extension
                    };
                default:
                    throw new NotSupportedException($"Compiler type {definition.Type} not supported.");
            }
        }
    }
}
