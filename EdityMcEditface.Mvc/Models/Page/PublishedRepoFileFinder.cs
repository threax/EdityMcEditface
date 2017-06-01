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
    public class PublishableRepoFileFinder : FileFinder
    {
        public PublishableRepoFileFinder(string projectPath, IFileFinderPermissions permissions, FileFinder next = null, string projectFilePath = "edity/edity.json")
            : base(projectPath, permissions, next, projectFilePath)
        {
        }

        private JsonSerializer serializer = JsonSerializer.CreateDefault();

        public override void PrepublishPage(String file)
        {
            bool goNext = true;
            var fullPath = NormalizePath(file);
            if (File.Exists(fullPath))
            {
                using (var repo = new Repository(Repository.Discover(fullPath)))
                {
                    var latestCommit = repo.Commits.QueryBy(file).FirstOrDefault();
                    if (latestCommit != null)
                    {
                        PublishedPageInfo publishInfo = LoadPublishInfo(file);

                        publishInfo.Sha = latestCommit.Commit.Sha;
                        WritePublishInfo(file, publishInfo);

                        goNext = false;
                    }
                }
            }

            if (goNext)
            {
                base.PrepublishPage(file);
            }
        }

        protected PublishedPageInfo LoadPublishInfo(string file)
        {
            PublishedPageInfo publishInfo = null;
            var publishInfoFile = GetPublishInfoFileName(file);
            //See if we can read the file
            try
            {
                var normalizedPath = NormalizePath(publishInfoFile);
                if (File.Exists(normalizedPath))
                {
                    //Always read file directly, this gets called during the ReadFile function call.
                    using (var reader = new JsonTextReader(new StreamReader(File.Open(normalizedPath, FileMode.Open, FileAccess.Read, FileShare.Read))))
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

        protected void WritePublishInfo(string file, PublishedPageInfo publishInfo)
        {
            using (var writer = new JsonTextWriter(new StreamWriter(WriteFile(GetPublishInfoFileName(file)))))
            {
                serializer.Serialize(writer, publishInfo);
            }
        }

        protected String GetPublishInfoFileName(String file)
        {
            return Path.ChangeExtension(file, ".publish");
        }
    }

    public class PublishedRepoFileFinder : PublishableRepoFileFinder
    {
        public PublishedRepoFileFinder(string projectPath, IFileFinderPermissions permissions, FileFinder next = null, string projectFilePath = "edity/edity.json")
            : base(projectPath, permissions, next, projectFilePath)
        {
        }

        protected override Stream OpenReadStream(String originalFile, String normalizedFile, bool isPublishable)
        {
            if (isPublishable)
            {
                PublishedPageInfo publishInfo = LoadPublishInfo(originalFile);
                if (publishInfo.Sha != null) //If we have publish info and it specifies an earlier published version, load that version
                {
                    using (var repo = new Repository(Repository.Discover(normalizedFile)))
                    {
                        var commit = repo.Lookup<Commit>(publishInfo.Sha);
                        var treeEntry = commit[originalFile.TrimStartingPathChars()];
                        var blob = treeEntry.Target as Blob;

                        return blob.GetContentStream();
                    }
                }

                throw new FileNotFoundException($"Cannot find draft version of {originalFile}.");
            }
            else
            {
                return base.OpenReadStream(originalFile, normalizedFile, isPublishable);
            }
        }
    }
}
