using Shared.Util;

namespace Server.DataSource
{
    public sealed class GlobalDataSource : Singleton<GlobalDataSource>
    {
        public AccountDataSource Account { get; } = new AccountDataSource();

        public void Initialize()
        {

        }
    }
}
