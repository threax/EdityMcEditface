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

        public bool SendPageToDraft(String file, String physicalFile, IFileFinder fileFinder)
        {
            if (File.Exists(physicalFile))
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

        public bool IsDraftedFile(string physicalFile)
        {
            var htmlFile = Path.ChangeExtension(physicalFile, "html");
            return File.Exists(htmlFile);
        }

        public IEnumerable<String> GetAllDraftables(IFileFinder fileFinder)
        {
            return fileFinder.EnumerateContentFiles("", "*.html");
        }

        public DraftInfo GetDraftStatus(String file, string physicalFile)
        {
            GitDraftInfo gitDraftInfo = LoadDraftInfo(physicalFile);

            //If the file has a sha, 
            if(gitDraftInfo.Sha != null)
            {
                using (var repo = new Repository(Repository.Discover(physicalFile)))
                {
                    //This is close, lookup the draft commit for the settings, css and js files also
                    var draftCommit = repo.Lookup<Commit>(gitDraftInfo.Sha);
                    if (draftCommit != null)
                    {
                        var latestCommit = repo.Commits.QueryBy(file.TrimStartingPathChars()).FirstOrDefault();
                        if (latestCommit != null)
                        {
                            var draftTime = draftCommit.Author.When.UtcDateTime;
                            var latestTime = latestCommit.Commit.Author.When.UtcDateTime;

                            DraftStatus status = latestTime > draftTime ? DraftStatus.UndraftedEdits : DraftStatus.UpToDate;
                            return new DraftInfo(draftCommit.Author.When.LocalDateTime, status, file);
                        }
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
