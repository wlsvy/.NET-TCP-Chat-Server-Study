using Client.Network;
using ImGuiNET;
using Shared.Gui;
using Shared.Logger;
using System;
using Client.Utils;
using System.Text;

namespace Client.Gui
{
    public sealed class LoginWindow : IImguiRenderer
    {
        public string WindowName => "Login";
        private bool m_IsOpen;
        bool IImguiRenderer.IsOpen { get => m_IsOpen; set => m_IsOpen = value; }
        private readonly byte[] m_IdBuffer = new byte[Util.MAX_INPUT_BUFFER_SIZE];
        private readonly byte[] m_PasswordBuffer = new byte[Util.MAX_INPUT_BUFFER_SIZE];

        void IImguiRenderer.Render()
        {
            var windowFlag = ImGuiWindowFlags.NoResize;
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
                ClientJobManager.I.ReserveJob(async () =>
                {
                    if (!ClientGuiWindow.I.ContainRenderer<CreateAccountWindow>())
                    {
                        ClientGuiWindow.I.AddImguiRenderer(new CreateAccountWindow());
                    }
                });
            }
            ImGui.PopID();

            ImGui.End();
        }

        private void OnLoginCallback(long accountId)
        {
            ServerConnection.I.LoginEvent -= OnLoginCallback;

            string msg;
            if (accountId == -1)
            {
                msg = "Login Account Fail";
            }
            else
            {
                msg = "Login Account Success";
            }

            ClientJobManager.I.ReserveJob(async () =>
            {
                ClientGuiWindow.I.CreatePopUp(msg);
            });
        }
    }
}
