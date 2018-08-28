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
        private ISiteBuilder currentBuilder;
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
        public void Compile(ISiteBuilder builder)
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
                //Do the build process on the thread pool, this way async will work correctly, this thread
                //will then wait for the result and process any exceptions that occur.
                var task = Task.Run(async () =>
                {
                    try
                    {
                        await builder.BuildSite();
                        return default(Exception);
                    }
                    catch(Exception ex)
                    {
                        return ex;
                    }
                });
                var runException = task.Result;
                //TODO: Do something to report the exception here.
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
