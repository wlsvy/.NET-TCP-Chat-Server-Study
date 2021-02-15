using Shared.Gui;
using Shared.Util;
using System;

namespace Client.Gui
{
    internal sealed class ClientGui : Singleton<ClientGui>
    {
        private VeldridWindow m_VeldridWindow;

        public void Initialize(VeldridWindow window)
        {
            m_VeldridWindow = window;
        }

        public bool Contain<TGuiRenderer>() where TGuiRenderer : IImguiRenderer
        {
            return m_VeldridWindow.ContainRenderer<TGuiRenderer>();
        }

        public void AddGui(IImguiRenderer renderer)
        {
            ClientJobManager.I.ReserveJob(async () =>
            {
                m_VeldridWindow.AddImguiRenderer(renderer);
            });
        }

        public void AddGuiIfNotExist(IImguiRenderer renderer)
        {
            ClientJobManager.I.ReserveJob(async () =>
            {
                if (!m_VeldridWindow.ContainRenderer(renderer))
                {
                    m_VeldridWindow.AddImguiRenderer(renderer);
                }
            });
        }

        public void AddGuiIfNotExist<TGuiRenderer>() where TGuiRenderer : class, IImguiRenderer, new()
        {
            ClientJobManager.I.ReserveJob(async () =>
            {
                if (!m_VeldridWindow.ContainRenderer<TGuiRenderer>())
                {
                    m_VeldridWindow.AddImguiRenderer(new TGuiRenderer());
                }
            });
        }

        public void RemoveGui(IImguiRenderer renderer)
        {
            ClientJobManager.I.ReserveJob(async () =>
            {
                m_VeldridWindow.TryRemoveRenderer(renderer);
            });
        }

        public void CreatePopUp(string message)
        {
            var popupWindow = new SimplePopUpMessageWindow(message);
            DoCreateWindow(popupWindow);
        }
        public void CreatePopUp(string message, Action onOk)
        {
            var popupWindow = new SimplePopUpMessageWindow(message, onOk);
            DoCreateWindow(popupWindow);
        }
        public void CreatePopUp(string message, Action onOk, Action onCancel)
        {
            var popupWindow = new SimplePopUpMessageWindow(message, onOk, onCancel);
            DoCreateWindow(popupWindow);
        }

        public override void Destroy()
        {
            base.Destroy();
            m_VeldridWindow.Dispose();
        }

        private void DoCreateWindow(SimplePopUpMessageWindow popupWindow)
        {
            popupWindow.OnClose += () =>
            {
                ClientJobManager.I.ReserveJob(async () =>
                {
                    m_VeldridWindow.TryRemoveRenderer(popupWindow);
                });
            };
            AddGui(popupWindow);
        }

        
    }
}
