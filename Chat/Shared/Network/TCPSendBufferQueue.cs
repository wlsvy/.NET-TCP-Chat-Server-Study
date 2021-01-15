using System;
using System.Collections.Generic;
using Shared.Util;
using System.Diagnostics;

namespace Shared.Network
{
    internal sealed class TcpSendBufferQueue
    {
        private readonly List<ArraySegment<byte>> m_List = new List<ArraySegment<byte>>(1024);
        private readonly Queue<ArraySegment<byte>> m_Queue = new Queue<ArraySegment<byte>>();

        public IList<ArraySegment<byte>> List => m_List;
        
        public void Add(ArraySegment<byte> sendBuffer)
        {
            if(m_List.Count < m_List.Capacity)
            {
                if(!m_Queue.IsEmpty())
                {
                    throw new Exception();
                }

                m_List.Add(sendBuffer);
            }
            else
            {
                m_Queue.Enqueue(sendBuffer);
            }
        }

        public void Clear()
        {
            m_Queue.Clear();
            m_List.Clear();
        }

        public void Remove(int bytesSent)
        {
            var sum = 0;
            var i = 0;
            while (i < m_List.Count && sum + m_List[i].Count <= bytesSent)
            {
                sum += m_List[i].Count;
                ++i;
            }

            Debug.Assert(sum <= bytesSent);
            Debug.Assert(!(sum < bytesSent) || i < m_List.Count && bytesSent - sum < m_List[i].Count);

            m_List.RemoveRange(0, i);
            bytesSent -= sum;

            if(0 < bytesSent)
            {
                var segment = m_List[0];
                m_List[0] = new ArraySegment<byte>(
                    segment.Array,
                    segment.Offset + bytesSent,
                    segment.Count - bytesSent);
            }

            while(0 < m_Queue.Count && m_List.Count < m_List.Capacity)
            {
                m_List.Add(m_Queue.Dequeue());
            }
        }

        public void TrimExcess()
        {
            Debug.Assert(m_Queue.IsEmpty() || m_List.Capacity <= m_List.Count);
            m_Queue.TrimExcess();
        }
    }
}
