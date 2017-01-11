using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface.Mvc.Models.Git
{
    public class SyncInfo
    {
        public int AheadBy { get; set; }

        public int BehindBy { get; set; }

        public IEnumerable<History> AheadHistory { get; set; }

        public IEnumerable<History> BehindHistory { get; set; }

        public bool HasUncomittedChanges { get; set; }
    }
}
