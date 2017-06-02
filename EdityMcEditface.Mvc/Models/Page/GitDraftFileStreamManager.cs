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
    public class GitDraftFileStreamManager : FileStreamManager
    {
        GitDraftManager publishedFileManager;
    
        public GitDraftFileStreamManager(GitDraftManager publishedFileManager)
        {
            this.publishedFileManager = publishedFileManager;
        }

        public override Stream OpenReadStream(String originalFile, String physicalFile)
        {
            if (publishedFileManager.IsDraftedFile(physicalFile))
            {
                GitDraftInfo publishInfo = publishedFileManager.LoadDraftInfo(physicalFile);
                if (publishInfo.Sha != null) //If we have publish info and it specifies an earlier published version, load that version
                {
                    return OpenRepoStream(physicalFile, publishInfo);
                }

                throw new FileNotFoundException($"Cannot find draft version of {originalFile}.");
            }
            else
            {
                return base.OpenReadStream(originalFile, physicalFile);
            }
        }

        public override void CopyFile(String source, string physicalSource, string physicalDest)
        {
            if(publishedFileManager.IsDraftedFile(physicalSource))
            {
                GitDraftInfo publishInfo = publishedFileManager.LoadDraftInfo(physicalSource);
                if (publishInfo.Sha != null) //If we have publish info and it specifies an earlier published version, load that version
                {
                    using (var destStream = File.Open(physicalDest, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        using (var srcStream = OpenRepoStream(physicalSource, publishInfo))
                        {
                            srcStream.CopyTo(destStream);
                        }
                    }
                }
                else
                {
                    throw new FileNotFoundException($"Cannot find draft version of {source}.");
                }
            }
            else
            {
                base.CopyFile(source, physicalSource, physicalDest);
            }
        }

        private static Stream OpenRepoStream(string physicalFile, GitDraftInfo publishInfo)
        {
            var repoPath = Path.GetFullPath(Repository.Discover(physicalFile) + "../");
            var fileInRepoPath = physicalFile.Substring(repoPath.Length);
            using (var repo = new Repository(repoPath))
            {
                var commit = repo.Lookup<Commit>(publishInfo.Sha);
                var treeEntry = commit[fileInRepoPath.TrimStartingPathChars()];
                var blob = treeEntry.Target as Blob;

                return blob.GetContentStream();
            }
        }
    }
}
