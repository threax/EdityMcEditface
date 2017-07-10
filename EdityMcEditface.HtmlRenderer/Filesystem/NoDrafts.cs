using System;
using System.Collections.Generic;
using System.Text;

namespace EdityMcEditface.HtmlRenderer.Filesystem
{
    public class NoDrafts : IDraftManager
    {
        public bool IsDraftedFile(String file, string normalizedFile)
        {
            return false;
        }

        public bool SendPageToDraft(String file, String physicalFile, IFileFinder fileFinder)
        {
            return false;
        }

        public IEnumerable<String> GetAllDraftables(IFileFinder fileFinder)
        {
            yield break;
        }

        public DraftInfo GetDraftStatus(String file, String physicalFile, IFileFinder fileFinder)
        {
            return new DraftInfo(null, DraftStatus.NeverDrafted, file);
        }

        public void PageErased(string file, string physicalPath)
        {
            
        }
    }
}
