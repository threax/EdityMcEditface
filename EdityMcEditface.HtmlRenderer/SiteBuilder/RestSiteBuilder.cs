using EdityMcEditface.HtmlRenderer.Compiler;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Threax.SharedHttpClient;

namespace EdityMcEditface.HtmlRenderer.SiteBuilder
{
    /// <summary>
    /// Build a website and deploy it to a rest endpoint.
    /// </summary>
    public class RestSiteBuilder : SiteBuilder
    {
        private SiteBuilderSettings settings;
        private DirectOutputSiteBuilder directOutput;
        private String baseOutputFolder;
        private String outputFullPath;
        private String zipOutputPath;
        private RemotePublishOptions publishOptions;
        private ISharedHttpClient httpClient;

        public RestSiteBuilder(RemotePublishOptions publishOptions, SiteBuilderSettings settings, IContentCompilerFactory contentCompilerFactory, IFileFinder fileFinder, ISharedHttpClient httpClient)
        {
            baseOutputFolder = Path.GetFullPath(settings.OutDir);
            outputFullPath = Path.GetFullPath(Path.Combine(baseOutputFolder, "azurezip"));
            zipOutputPath = Path.GetFullPath(Path.Combine(baseOutputFolder, "azurezip.zip"));
            settings.OutDir = outputFullPath;
            this.settings = settings;
            this.publishOptions = publishOptions ?? throw new InvalidOperationException("You must specify RemotePublishOptions in your project configuration to use the RestSiteBuilder.");
            this.httpClient = httpClient;
            directOutput = new DirectOutputSiteBuilder(settings, contentCompilerFactory, fileFinder);
        }

        public void BuildSite()
        {
            try
            {
                directOutput.BuildSite();

                //Check for zip file and erase it if it exists
                if (File.Exists(zipOutputPath))
                {
                    File.Delete(zipOutputPath);
                }

                ZipFile.CreateFromDirectory(outputFullPath, zipOutputPath);

                using (var file = File.Open(zipOutputPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var message = new HttpRequestMessage(new HttpMethod(publishOptions.Method), publishOptions.Host);
                    message.Content = new StreamContent(file);
                    if (publishOptions.User != null)
                    {
                        var auth = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{publishOptions.User}:{publishOptions.Password}"));
                        message.Headers.Authorization = new AuthenticationHeaderValue("Basic", auth);
                    }

                    //Method is async, but the builders run on a background thread, go ahead and run SendAsync on the thread pool.
                    //This does not need to be high performance, it is only happening in 1 thread.
                    var resultTask = Task.Run(async () => await httpClient.Client.SendAsync(message));
                    var result = resultTask.Result;
                    if (!result.IsSuccessStatusCode)
                    {
                        throw new InvalidOperationException($"{result.StatusCode} Error publishing site.");
                    }
                }
            }
            finally
            {

                //Erase the zip file
                if (File.Exists(zipOutputPath))
                {
                    File.Delete(zipOutputPath);
                }

                //Erase the output dir
                if (Directory.Exists(outputFullPath))
                {
                    DirectOutputSiteBuilder.MultiTryDirDelete(outputFullPath);
                }
            }
        }

        public void addPreBuildTask(BuildTask task)
        {
            directOutput.addPreBuildTask(task);
        }

        public void addPostBuildTask(BuildTask task)
        {
            directOutput.addPostBuildTask(task);
        }

        public Stream OpenOutputWriteStream(string file)
        {
            return directOutput.OpenOutputWriteStream(file);
        }

        public bool DoesOutputFileExist(string file)
        {
            return directOutput.DoesOutputFileExist(file);
        }

        public BuildProgress GetCurrentProgress()
        {
            return directOutput.GetCurrentProgress();
        }
    }
}
