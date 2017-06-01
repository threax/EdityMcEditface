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
            PublishedPageInfo publishInfo;
            var publishInfoFile = GetPublishInfoFileName(file);
            //See if we can read the file
            try
            {
                using (var reader = new JsonTextReader(new StreamReader(ReadFile(publishInfoFile))))
                {
                    publishInfo = serializer.Deserialize<PublishedPageInfo>(reader);
                }
            }
            catch (Exception)
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

        //this is working pretty well, but need to call this prepublishpage even if on main branch
        //also when loading files to copy, make sure you load older ones for pages, will need to alter base
        //file finder for this

        

        /// <summary>
        /// Load a page stack item as content, this will not attempt to derive a file name
        /// and will use what is passed in.
        /// </summary>
        /// <returns></returns>
        public override PageStackItem LoadPageStackContent(String path)
        {
            PublishedPageInfo publishInfo = LoadPublishInfo(path);
            if(publishInfo.Sha != null)
            {
                if (permissions.AllowRead(this, path))
                {
                    var realPath = NormalizePath(path);

                    using (var repo = new Repository(Repository.Discover(realPath)))
                    {
                        var commit = repo.Lookup<Commit>(publishInfo.Sha);
                        var treeEntry = commit[path];
                        var blob = treeEntry.Target as Blob;

                        using(var sr = new StreamReader(blob.GetContentStream()))
                        {
                            return loadPageStackFile(path, sr);
                        }
                    }
                }

                if (next != null)
                {
                    return next.LoadPageStackContent(path);
                }
                else
                {
                    throw new FileNotFoundException($"Cannot find page stack file {path}", path);
                }
            }
            else
            {
                throw new FileNotFoundException($"Cannot find file {path} in publish mode. No Sha set on .publish file or .publish file does not exist");
            }
        }
    }
}
