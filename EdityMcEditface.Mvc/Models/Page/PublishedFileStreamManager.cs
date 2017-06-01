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
    public class PublishedFileStreamManager : FileStreamManager
    {
        PublishedFileManager publishedFileManager;
    
        public PublishedFileStreamManager(PublishedFileManager publishedFileManager)
        {
            this.publishedFileManager = publishedFileManager;
        }

        public override Stream OpenReadStream(String originalFile, String normalizedFile)
        {
            if (publishedFileManager.IsPublishableFile(normalizedFile))
            {
                PublishedPageInfo publishInfo = publishedFileManager.LoadPublishInfo(normalizedFile);
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
                return base.OpenReadStream(originalFile, normalizedFile);
            }
        }

        public override void CopyFile(string source, string dest)
        {
            if(publishedFileManager.IsPublishableFile(source))
            {
                PublishedPageInfo publishInfo = publishedFileManager.LoadPublishInfo(source);
                if (publishInfo.Sha != null) //If we have publish info and it specifies an earlier published version, load that version
                {
                    using (var repo = new Repository(Repository.Discover(source)))
                    {
                        var commit = repo.Lookup<Commit>(publishInfo.Sha);
                        var treeEntry = commit[source.TrimStartingPathChars()];
                        var blob = treeEntry.Target as Blob;

                        using(var destStream = File.Open(dest, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            using(var srcStream = blob.GetContentStream())
                            {
                                srcStream.CopyTo(destStream);
                            }
                        }
                    }
                }

                throw new FileNotFoundException($"Cannot find draft version of {source}.");
            }
            else
            {
                base.CopyFile(source, dest);
            }
        }
    }
}
