using EdityMcEditface.HtmlRenderer;
using EdityMcEditface.HtmlRenderer.FileInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer.Compiler
{
    public class ContentCompilerFactory : IContentCompilerFactory
    {
        ITargetFileInfoProvider fileInfoProvider;

        public ContentCompilerFactory(ITargetFileInfoProvider fileInfoProvider)
        {
            this.fileInfoProvider = fileInfoProvider;
        }

        public List<IContentCompiler> CreateCompilers(IFileFinder fileFinder, String outDir, IEnumerable<CompilerDefinition> definition)
        {
            return new List<IContentCompiler>(definition.Select(d => CreateCompiler(fileFinder, outDir, d)));
        }

        public IContentCompiler CreateCompiler(IFileFinder fileFinder, String outDir, CompilerDefinition definition)
        {
            switch (definition.Type)
            {
                case CompilerTypes.Html:
                    return new HtmlCompiler(fileFinder, outDir, definition.Template, fileInfoProvider)
                    {
                        OutputExtension = definition.Extension
                    };
                case CompilerTypes.Json:
                    return new JsonCompiler(fileFinder, outDir, definition.Template, fileInfoProvider)
                    {
                        OutputExtension = definition.Extension
                    };
                default:
                    throw new NotSupportedException($"Compiler type {definition.Type} not supported.");
            }
        }
    }
}
