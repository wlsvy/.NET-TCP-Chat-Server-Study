using Shared.Util;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public sealed class ClientJobManager : Singleton<ClientJobManager>
    {
        private readonly ConcurrentQueue<Func<Task>> m_ReservedJobs = new ConcurrentQueue<Func<Task>>();

        public async Task Update()
        {
            while (m_ReservedJobs.TryDequeue(out var job))
            {
                await job.Invoke();
            }
        }

        public void ReserveJob(Func<Task> job)
        {
            m_ReservedJobs.Enqueue(job);
        }
    }
}
