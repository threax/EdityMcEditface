using AutoMapper;
using EdityMcEditface.HtmlRenderer;
using EdityMcEditface.HtmlRenderer.SiteBuilder;
using EdityMcEditface.Mvc.Models.Compiler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace EdityMcEditface.Mvc.Services
{
    public class CompileService : ICompileService
    {
        private Object locker = new object(); //Object to act as a lock
        private WorkQueue workQueue;
        private SiteBuilder currentBuilder;
        private IMapper mapper;
        private CompileProgress lastProgress = null;

        public CompileService(WorkQueue workQueue, IMapper mapper)
        {
            this.workQueue = workQueue;
            this.mapper = mapper;
        }

        /// <summary>
        /// Run the compiler.
        /// </summary>
        public void Compile(SiteBuilder builder)
        {
            workQueue.Fire(() =>
            {
                lock (locker)
                {
                    this.lastProgress = null;
                    this.currentBuilder = builder;
                }
                Stopwatch sw = new Stopwatch();
                sw.Start();
                builder.BuildSite();
                sw.Stop();
                lock (locker)
                {
                    this.lastProgress = mapper.Map<CompileProgress>(builder.GetCurrentProgress());
                    this.lastProgress.Completed = true;
                    this.currentBuilder = null;
                }
            });
        }

        /// <summary>
        /// Get the status of the current build.
        /// </summary>
        /// <returns></returns>
        public CompileProgress Progress()
        {
            lock (locker)
            {
                //If there is a last progress, return that, otherwise derive the current progress if we can
                //Last progress is cleared when we start a new build
                var progress = new CompileProgress();
                if (this.lastProgress != null)
                {
                    progress = this.lastProgress;
                }
                else if (this.currentBuilder != null)
                {
                    progress = mapper.Map(this.currentBuilder.GetCurrentProgress(), progress);
                }
                else
                {
                    progress.CurrentFile = 0;
                    progress.TotalFiles = int.MaxValue;
                }
                return progress;
            }
        }
    }
}
