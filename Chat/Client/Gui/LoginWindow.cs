using Client.Network;
using ImGuiNET;
using Shared.Gui;
using Shared.Logger;
using System;
using System.Text;

namespace Client.Gui
{
    public sealed class LoginWindow : IImguiRenderer
    {
        private const int MAX_INPUT_SIZE = 32;

        public string WindowName => "Login";
        private bool m_IsOpen;
        bool IImguiRenderer.IsOpen { get => m_IsOpen; set => m_IsOpen = value; }
        private readonly byte[] m_IdBuffer = new byte[MAX_INPUT_SIZE];
        private readonly byte[] m_PasswordBuffer = new byte[MAX_INPUT_SIZE];
        private bool m_CreateAccountPopup;

        void IImguiRenderer.Render()
        {
            var windowFlag = ImGuiWindowFlags.NoResize;
            ImGui.Begin(WindowName, windowFlag);

            ImGui.Text("Login");


            ImGui.InputText("ID", m_IdBuffer, MAX_INPUT_SIZE);
            ImGui.InputText("PW", m_PasswordBuffer, MAX_INPUT_SIZE, ImGuiInputTextFlags.Password);

            ImGui.PushID("Login_Button");
            if (ImGui.Button("Login"))
            {
                var index = GetZeroIndex(m_IdBuffer);
                var id = Encoding.UTF8.GetString(new ReadOnlySpan<byte>(m_IdBuffer, 0, index));
                index = GetZeroIndex(m_PasswordBuffer);
                var password = Encoding.UTF8.GetString(new ReadOnlySpan<byte>(m_PasswordBuffer, 0, index));
                Log.I.Debug($"id : {id}:::END, password : {password}:::END");

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

            int GetZeroIndex(byte[] arr)
            {
                for (int i = 0; i < MAX_INPUT_SIZE; i++)
                {
                    if(arr[i] == 0)
                    {
                        return i;
                    }
                }
                return MAX_INPUT_SIZE;
            }
        }

        private void OnLoginCallback(long accountId)
        {
            ServerConnection.I.LoginEvent -= OnLoginCallback;

        }

        private void OnCreateAccountCallback(long accountId)
        {
            ServerConnection.I.CreateAccountEvent -= OnCreateAccountCallback;

            string msg;
            if (accountId == -1)
            {
                msg = "로그인 실패";
            }
            else
            {
                msg = "로그인 성공";
            }

            ClientJobManager.I.ReserveJob(async () =>
            {
                ClientGuiWindow.I.CreatePopUp(msg);
            });
        }
    }
}
