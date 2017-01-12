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

        public List<IContentCompiler> CreateCompilers(String inDir, String outDir, String backupPath, IEnumerable<CompilerDefinition> definition)
        {
            return new List<IContentCompiler>(definition.Select(d => CreateCompiler(inDir, outDir, backupPath, d)));
        }

        public IContentCompiler CreateCompiler(String inDir, String outDir, String backupPath, CompilerDefinition definition)
        {
            var fileFinder = new FileFinder1(inDir, backupPath);
            switch (definition.Type)
            {
                case CompilerTypes.Html:
                    return new HtmlCompiler(fileFinder, outDir, definition.Template, definition.Settings)
                    {
                        OutputExtension = definition.Extension
                    };
                case CompilerTypes.Json:
                    return new JsonCompiler(fileFinder, outDir, definition.Template)
                    {
                        OutputExtension = definition.Extension
                    };
                default:
                    throw new NotSupportedException($"Compiler type {definition.Type} not supported.");
            }
        }
    }
}
