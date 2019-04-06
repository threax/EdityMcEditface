using EdityMcEditface.HtmlRenderer;
using EdityMcEditface.HtmlRenderer.Compiler;
using System;
using System.Collections.Generic;
using Xunit;

namespace Tests
{
    public class Tests
    {
        /// <summary>
        /// Test to see if compiler lists merge correctly.
        /// </summary>
        [Fact]
        public void MergeCompilers() 
        {
            EdityProject project1 = new EdityProject();
            project1.Compilers.Add(new CompilerDefinition()
            {
                Type = CompilerTypes.Html,
                Extension = ".html",
            });

            EdityProject project2 = new EdityProject();
            project2.Compilers.Add(new CompilerDefinition()
            {
                Type = CompilerTypes.Html,
                Extension = ".html"
            });

            project2.Compilers.Add(new CompilerDefinition()
            {
                Type = CompilerTypes.Html,
                Extension = ".print.html"
            });

            project2.Compilers.Add(new CompilerDefinition()
            {
                Type = CompilerTypes.Json,
                Extension = ".search.json",
            });

            EdityProject project3 = new EdityProject();

            project2.merge(project1);

            project3.merge(project2);

            Assert.True(project3.Compilers.Count == 3);
        }
    }
}
