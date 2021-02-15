using Client.Network;
using Client.Utils;
using ImGuiNET;
using Shared.Gui;
using Shared.Logger;

namespace Client.Gui
{
    internal sealed class CreateAccountWindow : IImguiRenderer
    {
        public string WindowName => "CreateAccount";
        private bool m_IsOpen = true;
        bool IImguiRenderer.IsOpen { get => m_IsOpen; set => m_IsOpen = value; }
        private readonly byte[] m_IdBuffer = new byte[Util.MAX_INPUT_BUFFER_SIZE];
        private readonly byte[] m_PasswordBuffer = new byte[Util.MAX_INPUT_BUFFER_SIZE];

        void IImguiRenderer.Render()
        {
            var windowFlag = ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize;
            ImGui.Begin(WindowName, windowFlag);

            ImGui.InputText("ID", m_IdBuffer, Util.MAX_INPUT_BUFFER_SIZE);
            ImGui.InputText("PW", m_PasswordBuffer, Util.MAX_INPUT_BUFFER_SIZE, ImGuiInputTextFlags.Password);

            ImGui.PushID("CreateAccount_Button");
            if (ImGui.Button("CreateAccount"))
            {
                var id = Util.GetImGuiInputText(m_IdBuffer);
                var password = Util.GetImGuiInputText(m_PasswordBuffer);
                Log.I.Debug($"id : {id}, password : {password}");

                ServerConnection.I.CreateAccountEvent += OnCreateAccountCallback;
                ServerConnection.I.PacketSender.SEND_CS_CreateAccount(id, password);
            }
            if (ImGui.Button("Cancel"))
            {
                ClientJobManager.I.ReserveJob(async () =>
                {
                    ClientGui.I.RemoveGui(this);
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
                msg = "Create Account Fail";
            }
            else
            {
                msg = "Create Account Success";
            }

            ClientJobManager.I.ReserveJob(async () =>
            {
                ClientGui.I.CreatePopUp(msg);
                ClientGui.I.RemoveGui(this);
            });
        }
    }
}
