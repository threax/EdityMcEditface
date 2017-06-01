using EdityMcEditface.HtmlRenderer;
using EdityMcEditface.HtmlRenderer.Filesystem;
using LibGit2Sharp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EdityMcEditface.Mvc.Models.Page
{
    public class PublishedFileManager : IPublishedFileManager
    {
        private JsonSerializer serializer = JsonSerializer.CreateDefault();

        public bool SendPageToDraft(String file, String normalizedPath)
        {
            if (File.Exists(normalizedPath))
            {
                using (var repo = new Repository(Repository.Discover(normalizedPath)))
                {
                    var latestCommit = repo.Commits.QueryBy(file).FirstOrDefault();
                    if (latestCommit != null)
                    {
                        PublishedPageInfo publishInfo = LoadPublishInfo(normalizedPath);

                        publishInfo.Sha = latestCommit.Commit.Sha;
                        WritePublishInfo(file, publishInfo);
                    }
                }
                return true;
            }
            return false;
        }

        public PublishedPageInfo LoadPublishInfo(string file)
        {
            PublishedPageInfo publishInfo = null;
            var publishInfoFile = GetPublishInfoFileName(file);
            //See if we can read the file
            try
            {
                if (File.Exists(publishInfoFile))
                {
                    //Always read file directly, this gets called during the ReadFile function call.
                    using (var reader = new JsonTextReader(new StreamReader(File.Open(publishInfoFile, FileMode.Open, FileAccess.Read, FileShare.Read))))
                    {
                        publishInfo = serializer.Deserialize<PublishedPageInfo>(reader);
                    }
                }
            }
            catch (Exception) { }

            if (publishInfo == null)
            {
                publishInfo = new PublishedPageInfo();
            }

            return publishInfo;
        }

        public void WritePublishInfo(string file, PublishedPageInfo publishInfo)
        {
            //write files directly.
            using (var writer = new JsonTextWriter(new StreamWriter(File.Open(GetPublishInfoFileName(file), FileMode.Open, FileAccess.Read, FileShare.Read))))
            {
                serializer.Serialize(writer, publishInfo);
            }
        }

        public String GetPublishInfoFileName(String file)
        {
            return Path.ChangeExtension(file, ".publish");
        }

        public bool IsPublishableFile(string normalizedFile)
        {
            var htmlFile = Path.ChangeExtension(normalizedFile, "html");
            return File.Exists(htmlFile);
        }
    }
}
