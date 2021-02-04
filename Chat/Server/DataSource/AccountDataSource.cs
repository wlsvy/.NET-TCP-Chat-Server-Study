using Shared.Util;
using System.Collections.Generic;

namespace Server.DataSource
{
    public sealed class AccountDataSource
    {
        private readonly Dictionary<string, string> m_AccountData = new Dictionary<string, string>();
        private readonly IdGenerator m_AccountIdGenerator = new IdGenerator(startNumber : 0);

        public long LoginAccount(string id, string password)
        {
            if(TryLogin(id, password))
            {
                return m_AccountIdGenerator.Generate();
            }
            return -1;
        }

        public long CreateAccount(string id, string password)
        {
            if(TryCreateAccount(id, password))
            {
                return m_AccountIdGenerator.Generate();
            }
            return -1;
        }

        private bool TryLogin(string id, string password)
        {
            if(!m_AccountData.TryGetValue(id, out var realPassword))
            {
                return false;
            }
            return realPassword.Equals(password);
        }

        private bool TryCreateAccount(string id, string password)
        {
            if (m_AccountData.ContainsKey(id))
            {
                return false;
            }
            m_AccountData.Add(id, password);
            return true;
        }
    }
}
