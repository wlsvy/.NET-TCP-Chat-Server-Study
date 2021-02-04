using Client.Network;
using ImGuiNET;
using Shared.Gui;
using Shared.Logger;
using System.Text;

namespace Client.Gui
{
    public sealed class CreateAccountWindow : IImguiRenderer
    {
        private const int MAX_INPUT_SIZE = 32;

        public string WindowName => "CreateAccount";
        private bool m_IsOpen;
        bool IImguiRenderer.IsOpen { get => m_IsOpen; set => m_IsOpen = value; }
        private readonly byte[] m_IdBuffer = new byte[MAX_INPUT_SIZE];
        private readonly byte[] m_PasswordBuffer = new byte[MAX_INPUT_SIZE];

        void IImguiRenderer.Render()
        {
            var windowFlag = ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize;
            ImGui.Begin(WindowName, windowFlag);

            ImGui.InputText("ID", m_IdBuffer, MAX_INPUT_SIZE);
            ImGui.InputText("PW", m_PasswordBuffer, MAX_INPUT_SIZE, ImGuiInputTextFlags.Password);

            ImGui.PushID("CreateAccount_Button");
            if (ImGui.Button("CreateAccount"))
            {
                var id = Encoding.UTF8.GetString(m_IdBuffer).Trim();
                var password = Encoding.UTF8.GetString(m_PasswordBuffer).Trim();
                Log.I.Debug($"id : {id}, password : {password}");

                ServerConnection.I.CreateAccountEvent += OnCreateAccountCallback;
                ServerConnection.I.PacketSender.SEND_CS_CreateAccount(id, password);
            }
            if (ImGui.Button("Cancel"))
            {
                ClientJobManager.I.ReserveJob(async () =>
                {
                    ClientGuiWindow.I.TryRemoveRenderer(this);
                });
            }
            ImGui.PopID();

            ImGui.End();
        }

        private void OnCreateAccountCallback(long accountId)
        {
            ServerConnection.I.CreateAccountEvent -= OnCreateAccountCallback;

            string msg;
            if(accountId == -1)
            {
                msg = "계정 생성 실패";
            }
            else
            {
                msg = "계정 생성 성공";
            }

            ClientJobManager.I.ReserveJob(async () =>
            {
                ClientGuiWindow.I.CreatePopUp(msg);
                ClientGuiWindow.I.TryRemoveRenderer(this);
            });
        }
    }
}
