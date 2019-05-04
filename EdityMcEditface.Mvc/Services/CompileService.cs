using AutoMapper;
using EdityMcEditface.HtmlRenderer;
using EdityMcEditface.HtmlRenderer.SiteBuilder;
using EdityMcEditface.Mvc.Auth;
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
        public void Compile(ISiteBuilder builder, IUserInfo compilingUser)
        {
            var userInfo = new BuilderUserInfo()
            {
                Email = compilingUser.Email,
                Name = compilingUser.PrettyUserName
            };

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
                        await builder.BuildSite(userInfo);
                        return default(Exception);
                    }
                    catch(Exception ex)
                    {
                        return ex;
                    }
                });
                var runException = task.Result; //This line has threading implications, don't move it
                sw.Stop();
                lock (locker)
                {
                    this.lastProgress = mapper.Map<CompileProgress>(builder.GetCurrentProgress());
                    this.lastProgress.Completed = true;
                    this.lastProgress.Success = runException == null; //If the run exception is null, the process was sucessful.
                    this.lastProgress.ErrorMessage = runException?.Message;
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
