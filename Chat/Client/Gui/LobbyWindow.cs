using ImGuiNET;
using Shared.Gui;
using System.Numerics;

namespace Client.Gui
{
    internal sealed class LobbyWindow : IImguiRenderer
    {
        public string WindowName => "Lobby";
        private bool m_IsOpen = true;
        bool IImguiRenderer.IsOpen { get => m_IsOpen; set => m_IsOpen = value; }

        void IImguiRenderer.Render()
        {
            ImGui.SetNextWindowSize(new Vector2(800, 600), ImGuiCond.FirstUseEver);
            ImGui.Begin("Lobby");
            if (ImGui.Button("Logout"))
            {
                ClientGui.I.VeldridWindow.TryRemoveRenderer(this);
                ClientGui.I.VeldridWindow.AddImguiRenderer(new LoginWindow());
            }

            ImGui.End();
        }
    }
}
