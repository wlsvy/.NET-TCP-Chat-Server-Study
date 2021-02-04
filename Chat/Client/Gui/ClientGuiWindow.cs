using Shared.Gui;
using Shared.Util;
using Shared.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Gui
{
    public sealed class ClientGuiWindow : VeldridWindow<ClientGuiWindow>
    {
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
        private void DoCreateWindow(SimplePopUpMessageWindow popupWindow)
        {
            popupWindow.OnClose += () =>
            {
                ClientGuiWindow.I.TryRemoveRenderer(popupWindow);
            };
            AddImguiRenderer(popupWindow);
        }
    }
}
