using Shared.Logger;
using System;
using System.Collections.Concurrent;

namespace Server
{
    public sealed class SessionManager
    {
        private readonly ConcurrentDictionary<long, Session> m_Sessions = new ConcurrentDictionary<long, Session>();

        public Session CreateSession(long id)
        {
            var session = new Session(id);
            if(!m_Sessions.TryAdd(id, session))
            {
                throw new ArgumentException($"{nameof(SessionManager)}.{nameof(this.CreateSession)} Session 생성 실패");
            }

            return session;
        }

        public bool TryGetSession(long id, out Session session)
        {
            return m_Sessions.TryGetValue(id, out session);
        }

        public void RemoveSession(long id)
        {
            if(!m_Sessions.TryRemove(id, out var session))
            {
                Log.I.Warn($"{nameof(SessionManager)}.{nameof(this.RemoveSession)} 유효하지 않은 {nameof(id)} : {id.ToString()}, 입니다.");
                return;
            }
            session.Dispose();
        }
    }
}
