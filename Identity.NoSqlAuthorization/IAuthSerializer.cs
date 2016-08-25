using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.NoSqlAuthorization
{
    public interface IAuthSerializer<T>
    {
        IEnumerable<T> Load();

        void Save(IEnumerable<T> users);
    }
}
