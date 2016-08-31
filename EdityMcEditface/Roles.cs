using Identity.NoSqlAuthorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdityMcEditface
{
    public class Roles
    {
        public const String EditPages = "EditPages";
        public const String UploadAnything = "UploadAnything";
        public const String Compile = "Compile";
        public const String Shutdown = "Shutdown";

        public static IEnumerable<NoSqlRole> Create()
        {
            yield return new NoSqlRole(EditPages);
            yield return new NoSqlRole(UploadAnything);
            yield return new NoSqlRole(Compile);
            yield return new NoSqlRole(Shutdown);
        }
    }
}
