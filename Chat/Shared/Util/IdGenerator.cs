using System.Threading;

namespace Shared.Util
{
    public sealed class IdGenerator
    {
        private long m_LastAssignedId;

        public IdGenerator(long startNumber = 0)
        {
            m_LastAssignedId = startNumber - 1;
        }

        public long Generate()
        {
            return Interlocked.Increment(ref m_LastAssignedId);
        }
    }
}
