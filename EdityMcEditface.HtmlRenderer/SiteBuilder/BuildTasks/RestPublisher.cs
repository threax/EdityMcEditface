using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Threax.SharedHttpClient;

namespace EdityMcEditface.HtmlRenderer.SiteBuilder.BuildTasks
{
    public class RestPublisher : BuildTask
    {
        ISharedHttpClient httpClient;
        RemotePublishOptions publishOptions;
        String outputPath;
        String zipOutputPath;

        public RestPublisher(RemotePublishOptions publishOptions, ISharedHttpClient httpClient, String outputPath)
        {
            this.httpClient = httpClient;
            this.publishOptions = publishOptions ?? throw new InvalidOperationException("You must specify RemotePublishOptions in your project configuration to use the RestSiteBuilder.");
            this.outputPath = outputPath;
            this.zipOutputPath = outputPath + ".zip"; //Just append .zip to the out dir, that will create the correct zip file in the same folder as the source folder.
        }

        public void execute()
        {
            try
            {
                //Check for zip file and erase it if it exists
                if (File.Exists(zipOutputPath))
                {
                    File.Delete(zipOutputPath);
                }

                ZipFile.CreateFromDirectory(outputPath, zipOutputPath);

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
                if (Directory.Exists(outputPath))
                {
                    DirectOutputSiteBuilder.MultiTryDirDelete(outputPath);
                }
            }
        }
    }
}
