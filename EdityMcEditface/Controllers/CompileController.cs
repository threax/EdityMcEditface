using EdityMcEditface.HtmlRenderer;
using EdityMcEditface.HtmlRenderer.Compiler;
using EdityMcEditface.HtmlRenderer.SiteBuilder;
using EdityMcEditface.Models.Compiler;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.Controllers
{
    [Route("edity/[controller]")]
    [Authorize(Roles=Roles.Compile)]
    public class CompileController : Controller
    {
        private SiteBuilder builder;
        private WorkQueue workQueue;

        public CompileController(SiteBuilder builder, WorkQueue workQueue)
        {
            this.builder = builder;
            this.workQueue = workQueue;
        }

        [HttpPost]
        public async Task<CompilerResult> Index()
        {
            Stopwatch sw = new Stopwatch();

            await workQueue.FireAsync(() =>
            {
                sw.Start();
                builder.BuildSite();
                sw.Stop();
            });

            return new CompilerResult()
            {
                ElapsedSeconds = sw.Elapsed.TotalSeconds
            };
        }
    }
}
