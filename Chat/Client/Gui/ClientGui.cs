using Shared.Gui;
using Shared.Util;
using System;

namespace Client.Gui
{
    internal sealed class ClientGui : Singleton<ClientGui>
    {
        public VeldridWindow VeldridWindow { get; }
        public ClientGui()
        {
            VeldridWindow = new VeldridWindow();
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
            VeldridWindow.Dispose();
        }

        private void DoCreateWindow(SimplePopUpMessageWindow popupWindow)
        {
            popupWindow.OnClose += () =>
            {
                ClientJobManager.I.ReserveJob(async () =>
                {
                    VeldridWindow.TryRemoveRenderer(popupWindow);
                });
            };
            ClientJobManager.I.ReserveJob(async () =>
            {
                VeldridWindow.AddImguiRenderer(popupWindow);
            });
        }

        
    }
}
