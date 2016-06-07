using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viewer
{
    /// <summary>
    /// This class allows you to reassign a disposable in a using block by assiging a
    /// value to this class instead. This will just dispose what is attached to it
    /// when this class gets disposed, it does nothing about anything that is attached
    /// and removed before this class is disposed. Null targets are supported.
    /// </summary>
    class DisposeShim<T> : IDisposable 
        where T : IDisposable
    {
        public DisposeShim()
        {
            this.Target = default(T);
        }

        public DisposeShim(T target)
        {
            this.Target = target;
        }

        /// <summary>
        /// Dispose the target object if it is not null.
        /// </summary>
        public void Dispose()
        {
            if (Target != null)
            {
                Target.Dispose();
            }
        }

        /// <summary>
        /// The target object to dispose. Can be null to do nothing.
        /// </summary>
        public T Target { get; set; }
    }
}
