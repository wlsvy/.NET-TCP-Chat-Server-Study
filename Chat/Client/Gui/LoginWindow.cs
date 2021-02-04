using Client.Network;
using ImGuiNET;
using Shared.Gui;
using Shared.Logger;
using System.Text;

namespace Client.Gui
{
    public sealed class LoginWindow : IImguiRenderer
    {
        private const int MAX_INPUT_SIZE = 32;

        public string WindowName => "Login";
        private bool m_IsOpen;
        bool IImguiRenderer.IsOpen { get => m_IsOpen; set => m_IsOpen = value; }
        private readonly ServerConnection m_Connection;
        private readonly byte[] m_IdBuffer = new byte[MAX_INPUT_SIZE];
        private readonly byte[] m_PasswordBuffer = new byte[MAX_INPUT_SIZE];
        private bool m_CreateAccountPopup;
        public LoginWindow(ServerConnection connection)
        {
            m_Connection = connection;
        }

        void IImguiRenderer.Render()
        {
            var windowFlag = ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize;
            ImGui.Begin(WindowName, windowFlag);

            ImGui.Text("Login");


            ImGui.InputText("ID", m_IdBuffer, MAX_INPUT_SIZE);
            ImGui.InputText("PW", m_PasswordBuffer, MAX_INPUT_SIZE, ImGuiInputTextFlags.Password);

            ImGui.PushID("Login_Button");
            if (ImGui.Button("Login"))
            {
                var id = Encoding.UTF8.GetString(m_IdBuffer);
                var password = Encoding.UTF8.GetString(m_PasswordBuffer);
                Log.I.Debug($"id : {id}, password : {password}");

                m_Connection.PacketSender.SEND_CS_Login(id, password);
            }
            ImGui.SameLine();
            if(ImGui.Button("Create Account"))
            {
                if (!ClientGuiWindow.I.ContainRenderer<CreateAccountWindow>())
                {
                    ClientGuiWindow.I.AddImguiRenderer(new CreateAccountWindow(m_Connection));
                }
            }
            ImGui.PopID();

            ImGui.End();
        }
    }
}
