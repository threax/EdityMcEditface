using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EdityMcEditface.HtmlRenderer
{
    public class WorkQueue
    {
        BlockingCollection<Action> queue = new BlockingCollection<Action>();

        public WorkQueue()
        {
            var t = new Thread(arg =>
            {
                while (true)
                {
                    var action = queue.Take();
                    action();
                }
            });
            t.IsBackground = true;
            t.Start();
        }

        public void Fire(Action action)
        {
            queue.Add(action);
        }

        public async Task FireAsync(Action action)
        {
            var complete = new TaskCompletionSource<int>();
            queue.Add(() =>
            {
                action();
                complete.SetResult(0);
            });
            await complete.Task;
        }
    }
}
