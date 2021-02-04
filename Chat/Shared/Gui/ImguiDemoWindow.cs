using ImGuiNET;

namespace Shared.Gui
{
    public class ImguiDemoWindow : IImguiRenderer
    {
        private bool m_IsOpen = true;
        string IImguiRenderer.WindowName => "ImguiDemo";
        bool IImguiRenderer.IsOpen { get => m_IsOpen; set => m_IsOpen = value; }

        void IImguiRenderer.Render()
        {
            ImGui.ShowDemoWindow(ref m_IsOpen);
        }
    }
}
