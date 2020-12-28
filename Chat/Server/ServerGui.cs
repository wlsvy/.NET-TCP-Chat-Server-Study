using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;
using System;
using ImGuiNET;

namespace Server
{
    public sealed class ServerGui : IDisposable
    {
        public bool IsWindowExist
        {
            get
            {
                if(m_Window == null)
                {
                    return false;
                }
                return m_Window.Exists;
            }
        }

        private Sdl2Window m_Window;
        private GraphicsDevice m_GraphicsDevice;
        private CommandList m_CommandList;
        private ImguiManager m_ImguiManager;

        private bool m_IsDisposed = false;

        public void Initialize()
        {
            VeldridStartup.CreateWindowAndGraphicsDevice(
                new WindowCreateInfo(
                    (int)GuiUtil.WINDOW_INTIAL_POSITION.X, 
                    (int)GuiUtil.WINDOW_INTIAL_POSITION.Y, 
                    (int)GuiUtil.WINDOW_INITAL_SIZE.X, 
                    (int)GuiUtil.WINDOW_INITAL_SIZE.Y, 
                    WindowState.Normal, 
                    "ImGui.NET Sample Program"),
                new GraphicsDeviceOptions(true, null, true, ResourceBindingModel.Improved, true, true),
                out m_Window,
                out m_GraphicsDevice);

            m_Window.Resized += OnWindowResize;

            m_CommandList = m_GraphicsDevice.ResourceFactory.CreateCommandList();
            m_ImguiManager = new ImguiManager(
                m_GraphicsDevice,
                m_GraphicsDevice.MainSwapchain.Framebuffer.OutputDescription,
                m_Window.Width,
                m_Window.Height);
        }

        public void Update(int deltaMSec)
        {
            OnRenderBegin(deltaMSec);

            ImGui.Begin("Hello World");
            ImGui.ShowDemoWindow();

            Submit();
        }

        public void Dispose()
        {
            if (m_IsDisposed)
            {
                return;
            }

            m_GraphicsDevice?.WaitForIdle();
            m_ImguiManager?.Dispose();
            m_CommandList?.Dispose();
            m_GraphicsDevice?.Dispose();
            if(m_Window != null)
            {
                m_Window.Resized -= OnWindowResize;
                m_Window.Close();
            }
            m_IsDisposed = true;
        }

        private void OnWindowResize()
        {
            m_ImguiManager.OnWindowResize(m_Window.Width, m_Window.Height);
            m_GraphicsDevice.MainSwapchain.Resize((uint)m_Window.Width, (uint)m_Window.Height);
        }

        private void OnRenderBegin(int deltaMSec)
        {
            InputSnapshot snapshot = m_Window.PumpEvents();
            m_ImguiManager.OnFrameBegin(deltaMSec * 0.001f, snapshot);

            m_CommandList.Begin();

            m_CommandList.SetFramebuffer(m_GraphicsDevice.MainSwapchain.Framebuffer);
            m_CommandList.ClearColorTarget(0, GuiUtil.WINDOW_BACKGROUND_COLOR);
        }

        private void Submit()
        {
            m_ImguiManager.SubmitUi(m_GraphicsDevice, m_CommandList);
            m_CommandList.End();

            m_GraphicsDevice.SubmitCommands(m_CommandList);
            m_GraphicsDevice.SwapBuffers(m_GraphicsDevice.MainSwapchain);
        }
    }
}
