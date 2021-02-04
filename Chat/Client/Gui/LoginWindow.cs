using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using Shared.Gui;
using Shared.Logger;

namespace Client.Gui
{
    public sealed class LoginWindow : IImguiRenderer
    {
        private bool m_IsOpen;
        public string WindowName => "Login";
        bool IImguiRenderer.IsOpen { get => m_IsOpen; set => m_IsOpen = value; }

        void IImguiRenderer.Render()
        {
            var windowFlag = ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize;
            ImGui.Begin(WindowName, windowFlag);

            ImGui.Text("Login");

            const int maxInputSize = 16;
            var idBuffer = new byte[maxInputSize];
            var passwordBuffer = new byte[maxInputSize];

            ImGui.InputText("ID", idBuffer, maxInputSize);
            ImGui.InputText("PW", passwordBuffer, maxInputSize, ImGuiInputTextFlags.Password);

            ImGui.PushID("Login_Button");
            if (ImGui.Button("Login"))
            {
                var id = Encoding.UTF8.GetString(idBuffer);
                var password = Encoding.UTF8.GetString(passwordBuffer);
                Log.I.Debug($"id : {id}, password : {password}");
            }
            ImGui.SameLine();
            if(ImGui.Button("Create Account"))
            {

            }
            ImGui.PopID();

            ImGui.End();
        }
    }
}
