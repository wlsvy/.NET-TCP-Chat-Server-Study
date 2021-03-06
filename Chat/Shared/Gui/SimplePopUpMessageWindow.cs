﻿using ImGuiNET;
using Shared.Util;
using System;
using System.Numerics;

namespace Shared.Gui
{
    public sealed class SimplePopUpMessageWindow : IImguiRenderer
    {
        private static IdGenerator s_IdGenerator = new IdGenerator(startNumber: 0);

        public event Action OnClose;
        private bool m_IsOpen = true;
        string IImguiRenderer.WindowName => "Pop_Up";
        bool IImguiRenderer.IsOpen { get => m_IsOpen; set => m_IsOpen = value; }
        private readonly long m_Id;
        private string m_Message;
        private Action m_OkCallback;
        private Action m_CancelCallback;

        public SimplePopUpMessageWindow(string message)
        {
            m_Id = s_IdGenerator.Generate();
            m_Message = message;
        }
        public SimplePopUpMessageWindow(string message, Action onOk) : this(message)
        {
            m_OkCallback = onOk;
        }
        public SimplePopUpMessageWindow(string message, Action onOk, Action onCancel) : this(message, onOk)
        {
            m_CancelCallback = onCancel;
        }

        void IImguiRenderer.Render()
        {
            ImGui.SetNextWindowSize(new Vector2(256, 64));
            ImGui.Begin($"popupMessage {m_Id}", ImGuiWindowFlags.NoTitleBar);
            ImGui.PushID(m_Id.ToString());

            ImGui.Text(m_Message);
            ImGui.NewLine();
            if (ImGui.Button($"Ok"))
            {
                m_OkCallback?.Invoke();
                OnClose?.Invoke();
            }
            if(m_CancelCallback != null 
                && ImGui.Button($"Cancel"))
            {
                m_CancelCallback.Invoke();
                OnClose?.Invoke();
            }

            ImGui.PopID();
            ImGui.End();
        }
    }
}
