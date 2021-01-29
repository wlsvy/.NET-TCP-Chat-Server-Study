using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Shared.Protocol
{
    public abstract class PacketProcessorBase
    {
        public struct Barrier
        {
            private const int LOCKED = 1;
            private const int FREE = 0;

            private int m_Value;

            public bool TryEnter()
            {
                if(Interlocked.CompareExchange(ref m_Value, LOCKED, FREE) == FREE)
                {
                    return true;
                }
                return false;
            }

            public bool TryExit()
            {
                if (Interlocked.CompareExchange(ref m_Value, FREE, LOCKED) == LOCKED)
                {
                    return true;
                }
                return false;
            }
        }

        private readonly ConcurrentQueue<Func<Task>> m_HandlerQueue;
        private readonly Barrier m_Barrier;

        public PacketProcessorBase()
        {
            m_HandlerQueue = new ConcurrentQueue<Func<Task>>();
        }

        //TODO 여기 레이스 컨디션 테스트 해보기
        protected void RunOrReserveHandler(Func<Task> handler)
        {
            m_HandlerQueue.Enqueue(handler);
            _ = ProcessHandlers();
        }

        private async Task ProcessHandlers()
        {
            if (!m_Barrier.TryEnter())
            {
                return;
            }

            while (m_HandlerQueue.TryDequeue(out var handler))
            {
                await handler.Invoke();
            }
            m_Barrier.TryExit();
        }


    }
}
