using EdityMcEditface.HtmlRenderer.SiteBuilder;
using EdityMcEditface.Mvc.Models.Compiler;

namespace EdityMcEditface.Mvc.Services
{
    public interface ICompileService
    {
        /// <summary>
        /// Run the compiler.
        /// </summary>
        /// <returns>The time statistics when the compilation is complete.</returns>
        void Compile(ISiteBuilder builder);

        /// <summary>
        /// Get the status of the current build.
        /// </summary>
        /// <returns></returns>
        CompileProgress Progress();
    }
}