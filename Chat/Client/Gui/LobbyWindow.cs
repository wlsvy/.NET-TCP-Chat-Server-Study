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

            var guiWindowSize = ImGui.GetContentRegionMax();
            var guiWidth = guiWindowSize.X;
            var guiHeight = guiWindowSize.Y;

            ImGui.BeginChildFrame((uint)"Chatting Room List".GetHashCode(), new Vector2(guiWidth * 0.9f, guiHeight * 0.7f));

            ImGui.EndChildFrame();
            ImGui.NewLine();
            if (ImGui.Button("Logout"))
            {
                ClientGui.I.RemoveGui(this);
                ClientGui.I.CreatePopUp("Logout", 
                    onOk: () => ClientGui.I.AddGuiIfNotExist<LoginWindow>());
            }

            ImGui.End();
        }
    }
}
