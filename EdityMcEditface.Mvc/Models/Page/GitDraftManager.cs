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
    public class GitDraftManager : IDraftManager
    {
        private JsonSerializer serializer = JsonSerializer.CreateDefault();
        private IPathPermissions draftIdentfier;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="draftIdentfier">The path permissions to use to determine if a path can be drafted. Blacklist paths you don't want to support drafting.</param>
        public GitDraftManager(IPathPermissions draftIdentfier)
        {
            this.draftIdentfier = draftIdentfier;
        }

        public bool SendPageToDraft(String file, String physicalFile, IFileFinder fileFinder)
        {
            if (draftIdentfier.AllowFile(file, physicalFile) && File.Exists(physicalFile))
            {
                using (var repo = new Repository(Repository.Discover(physicalFile)))
                {
                    var latestCommit = repo.Commits.FirstOrDefault();
                    if (latestCommit != null)
                    {
                        GitDraftInfo publishInfo = LoadDraftInfo(physicalFile);

                        publishInfo.Sha = latestCommit.Sha;

                        using (var writer = new JsonTextWriter(new StreamWriter(fileFinder.WriteFile(GetDraftInfoFileName(file)))))
                        {
                            serializer.Serialize(writer, publishInfo);
                        }
                    }
                }
                return true;
            }
            return false;
        }

        public GitDraftInfo LoadDraftInfo(string file)
        {
            GitDraftInfo draftInfo = null;
            var draftInfoFile = GetDraftInfoFileName(file);
            //See if we can read the file
            try
            {
                if (File.Exists(draftInfoFile))
                {
                    //Always read file directly, this gets called during the ReadFile function call.
                    using (var reader = new JsonTextReader(new StreamReader(File.Open(draftInfoFile, FileMode.Open, FileAccess.Read, FileShare.Read))))
                    {
                        draftInfo = serializer.Deserialize<GitDraftInfo>(reader);
                    }
                }
            }
            catch (Exception) { }

            if (draftInfo == null)
            {
                draftInfo = new GitDraftInfo();
            }

            return draftInfo;
        }

        public String GetDraftInfoFileName(String file)
        {
            return Path.ChangeExtension(file, ".draft");
        }

        public bool IsDraftedFile(String file, string physicalFile)
        {
            var html = Path.ChangeExtension(file, "html");
            var physicalHtml = Path.ChangeExtension(physicalFile, "html");
            return draftIdentfier.AllowFile(html, physicalHtml) && File.Exists(physicalHtml);
        }

        public IEnumerable<String> GetAllDraftables(IFileFinder fileFinder)
        {
            return fileFinder.EnumerateContentFiles("", "*.html", SearchOption.AllDirectories);
        }

        public DraftInfo GetDraftStatus(String file, string physicalFile, IFileFinder fileFinder)
        {
            GitDraftInfo gitDraftInfo = LoadDraftInfo(physicalFile);

            //If the file has a sha, 
            if (gitDraftInfo.Sha != null)
            {
                var repoPath = Path.GetFullPath(Repository.Discover(physicalFile) + "/..");
                using (var repo = new Repository(repoPath))
                {
                    //This is close, lookup the draft commit for the settings, css and js files also
                    var draftCommit = repo.Lookup<Commit>(gitDraftInfo.Sha);
                    if (draftCommit != null)
                    {
                        var compare = repo.Diff.Compare<TreeChanges>(draftCommit.Tree, repo.Head.Tip.Tree);

                        foreach (var contentFile in new String[] { file }.Concat(fileFinder.GetPageContentFiles(file)))
                        {
                            if (compare.Any(i => i.Path == contentFile))
                            {
                                return new DraftInfo(draftCommit.Author.When.LocalDateTime, DraftStatus.UndraftedEdits, file);
                            }
                        }

                        //If we got here the draft is up to date
                        return new DraftInfo(draftCommit.Author.When.LocalDateTime, DraftStatus.UpToDate, file);
                    }
                }
            }

            return null;
        }

        public void PageErased(string file, string physicalPath)
        {
            var draftFile = GetDraftInfoFileName(physicalPath);
            if (draftFile != null && File.Exists(draftFile))
            {
                File.Delete(draftFile);
            }
        }
    }
}
