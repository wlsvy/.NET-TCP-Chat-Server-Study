using Client.Network;
using Client.Utils;
using ImGuiNET;
using Shared.Gui;
using Shared.Logger;
using System.Numerics;

namespace Client.Gui
{
    public sealed class LoginWindow : IImguiRenderer
    {
        public string WindowName => "Login";
        private bool m_IsOpen = true;
        bool IImguiRenderer.IsOpen { get => m_IsOpen; set => m_IsOpen = value; }
        private readonly byte[] m_IdBuffer = new byte[Util.MAX_INPUT_BUFFER_SIZE];
        private readonly byte[] m_PasswordBuffer = new byte[Util.MAX_INPUT_BUFFER_SIZE];

        void IImguiRenderer.Render()
        {
            var windowFlag = ImGuiWindowFlags.NoResize;
            ImGui.SetNextWindowSize(new Vector2(512, 128), ImGuiCond.FirstUseEver);
            ImGui.Begin(WindowName, windowFlag);

            ImGui.Text("Login");


            ImGui.InputText("ID", m_IdBuffer, Util.MAX_INPUT_BUFFER_SIZE);
            ImGui.InputText("PW", m_PasswordBuffer, Util.MAX_INPUT_BUFFER_SIZE, ImGuiInputTextFlags.Password);

            ImGui.PushID("Login_Button");
            if (ImGui.Button("Login"))
            {
                var id = Util.GetImGuiInputText(m_IdBuffer);
                var password = Util.GetImGuiInputText(m_PasswordBuffer);
                Log.I.Debug($"id : {id}, password : {password}");

                ServerConnection.I.LoginEvent += OnLoginCallback;
                ServerConnection.I.PacketSender.SEND_CS_Login(id, password);
            }
            ImGui.SameLine();
            if(ImGui.Button("Create Account"))
            {
                ClientGui.I.AddGuiIfNotExist<CreateAccountWindow>();
            }
            ImGui.PopID();

            ImGui.End();
        }

        private void OnLoginCallback(long accountId)
        {
            ServerConnection.I.LoginEvent -= OnLoginCallback;

            if (accountId == -1)
            {
                ClientGui.I.CreatePopUp("Login Account Fail");

            }
            else
            {
                ClientGui.I.RemoveGui(this);
                ClientGui.I.CreatePopUp("Login Account Success",
                    onOk: () => ClientGui.I.AddGuiIfNotExist<LobbyWindow>());
            }
        }
    }
}
