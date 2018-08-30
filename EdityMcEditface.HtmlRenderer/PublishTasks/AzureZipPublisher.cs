using EdityMcEditface.HtmlRenderer.SiteBuilder;
using EdityMcEditface.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Threax.SharedHttpClient;

namespace EdityMcEditface.PublishTasks
{
    public class AzureZipPublisher : IPublishTask
    {
        ISharedHttpClient httpClient;
        AzurePublishOptions publishOptions;
        String outputPath;
        String zipOutputPath;

        public AzureZipPublisher(AzurePublishOptions publishOptions, ISharedHttpClient httpClient, String outputPath)
        {
            this.httpClient = httpClient;
            this.publishOptions = publishOptions ?? throw new InvalidOperationException("You must specify AzureZipOptions in your configuration to use the AzureZipPublisher.");
            this.outputPath = outputPath;
            this.zipOutputPath = outputPath + ".zip"; //Just append .zip to the out dir, that will create the correct zip file in the same folder as the source folder.
        }

        public async Task Execute(BuildEventArgs args)
        {
            try
            {
                args.Tracker.AddMessage("Creating deployment zip.");

                //Check for zip file and erase it if it exists
                if (File.Exists(zipOutputPath))
                {
                    File.Delete(zipOutputPath);
                }

                ZipFile.CreateFromDirectory(outputPath, zipOutputPath);

                args.Tracker.AddMessage("Uploading zip to Azure.");

                using (var file = File.Open(zipOutputPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var auth = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{publishOptions.User}:{publishOptions.Password}"));
                    var message = new HttpRequestMessage(HttpMethod.Post, $"https://{publishOptions.SiteName}.scm.azurewebsites.net/api/zipdeploy?isAsync=true");
                    message.Content = new StreamContent(file);
                    message.Headers.Authorization = new AuthenticationHeaderValue("Basic", auth);
                    var response = await httpClient.Client.SendAsync(message);
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"Error uploading file. Message: {await response.Content.ReadAsStringAsync()}");
                    }

                    args.Tracker.AddMessage("Starting deployment on Azure service.");

                    var jsonSerializer = JsonSerializer.CreateDefault();
                    var pollLocation = response.Headers.Location;
                    bool poll = true;
                    String lastMessage = null;
                    while (poll)
                    {
                        Thread.Sleep(500);

                        var pollMessage = new HttpRequestMessage(HttpMethod.Get, pollLocation);
                        pollMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", auth);
                        var pollResponse = await httpClient.Client.SendAsync(pollMessage);
                        if (!pollResponse.IsSuccessStatusCode)
                        {
                            throw new Exception($"An error occured checking the deployment status on Azure. Message: {await pollResponse.Content.ReadAsStringAsync()}");
                        }

                        AzureStatus status;
                        var json = await pollResponse.Content.ReadAsStringAsync();
                        using (var stream = await pollResponse.Content.ReadAsStreamAsync())
                        using (var reader = new StreamReader(stream))
                        using (var jsonReader = new JsonTextReader(reader))
                        {
                            status = jsonSerializer.Deserialize<AzureStatus>(jsonReader);
                        }

                        if(!String.IsNullOrEmpty(status.progress) && status.progress != lastMessage)
                        {
                            lastMessage = status.progress;
                            args.Tracker.AddMessage(lastMessage);
                        }

                        poll = !status.complete;
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
                    IOExtensions.MultiTryDirDelete(outputPath);
                }
            }
        }
    }
}
