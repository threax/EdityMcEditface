using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.NoSqlAuthorization
{
    public class InMemoryStaticAuthSerializer<T> : IAuthSerializer<T>
    {
        public Func<IEnumerable<T>> loadFunc;

        public InMemoryStaticAuthSerializer(Func<IEnumerable<T>> loadFunc)
        {
            this.loadFunc = loadFunc;
        }

        public IEnumerable<T> Load()
        {
            return loadFunc();
        }

        public void Save(IEnumerable<T> users)
        {
            
        }
    }
}
