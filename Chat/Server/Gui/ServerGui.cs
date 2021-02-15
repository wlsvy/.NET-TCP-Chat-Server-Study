using Shared.Gui;
using Shared.Util;

namespace Server.Gui
{
    internal sealed class ServerGui : Singleton<ServerGui>
    {
        public VeldridWindow VeldridWindow { get; }

        public ServerGui()
        {
            VeldridWindow = new VeldridWindow();
        }
    }
}
