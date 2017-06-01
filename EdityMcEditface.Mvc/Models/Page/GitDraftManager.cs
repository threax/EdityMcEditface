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

        public bool SendPageToDraft(String normalizedPath)
        {
            if (File.Exists(normalizedPath))
            {
                using (var repo = new Repository(Repository.Discover(normalizedPath)))
                {
                    var latestCommit = repo.Commits.FirstOrDefault();
                    if (latestCommit != null)
                    {
                        DraftInfo publishInfo = LoadDraftInfo(normalizedPath);

                        publishInfo.Sha = latestCommit.Sha;
                        WriteDraftInfo(normalizedPath, publishInfo);
                    }
                }
                return true;
            }
            return false;
        }

        public DraftInfo LoadDraftInfo(string file)
        {
            DraftInfo publishInfo = null;
            var publishInfoFile = GetDraftInfoFileName(file);
            //See if we can read the file
            try
            {
                if (File.Exists(publishInfoFile))
                {
                    //Always read file directly, this gets called during the ReadFile function call.
                    using (var reader = new JsonTextReader(new StreamReader(File.Open(publishInfoFile, FileMode.Open, FileAccess.Read, FileShare.Read))))
                    {
                        publishInfo = serializer.Deserialize<DraftInfo>(reader);
                    }
                }
            }
            catch (Exception) { }

            if (publishInfo == null)
            {
                publishInfo = new DraftInfo();
            }

            return publishInfo;
        }

        public void WriteDraftInfo(string file, DraftInfo publishInfo)
        {
            //write files directly.
            using (var writer = new JsonTextWriter(new StreamWriter(File.Open(GetDraftInfoFileName(file), FileMode.Create, FileAccess.Write, FileShare.None))))
            {
                serializer.Serialize(writer, publishInfo);
            }
        }

        public String GetDraftInfoFileName(String file)
        {
            return Path.ChangeExtension(file, ".draft");
        }

        public bool IsDraftedFile(string normalizedFile)
        {
            var htmlFile = Path.ChangeExtension(normalizedFile, "html");
            return File.Exists(htmlFile);
        }
    }
}
