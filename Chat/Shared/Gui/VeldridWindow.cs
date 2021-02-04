using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;
using Shared.Util;
using System.Linq;
using System;
using ImGuiNET;
using Shared.Logger;
using System.Collections.Generic;

namespace Shared.Gui
{
    public abstract class VeldridWindow<T> : Singleton<T> where T : class, new()
    {
        public bool IsWindowExist
        {
            get
            {
                if (m_Window == null)
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
        private List<IImguiRenderer> m_UiRenderers = new List<IImguiRenderer>();

        private bool m_IsDisposed = false;

        public void Open()
        {
            try
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
            catch (Exception e)
            {
                Log.I.Error("Veldrid Window 열기 실패", e);
                Destroy();
            }

        }

        public virtual void Update(int deltaMSec)
        {
            OnRenderBegin(deltaMSec);

            RenderMainMenuBar();
            foreach (var renderer in m_UiRenderers)
            {
                if (renderer.IsOpen)
                {
                    renderer.Render();
                }
            }
            Submit();
        }

        public void AddImguiRenderer(IImguiRenderer renderer)
        {
            m_UiRenderers.Add(renderer);
        }
        public bool ContainRenderer<TRenderer>() where TRenderer : IImguiRenderer
        {
            foreach (var r in m_UiRenderers)
            {
                if (r is TRenderer target)
                {
                    return true;
                }
            }
            return false;
        }
        public bool TryGetRenderer<TRenderer>(out TRenderer renderer) where TRenderer : IImguiRenderer
        {
            foreach (var r in m_UiRenderers)
            {
                if (r is TRenderer target)
                {
                    renderer = target;
                    return true;
                }
            }
            renderer = default;
            return false;
        }
        public bool TryRemoveRenderer(IImguiRenderer renderer)
        {
            var count = m_UiRenderers.Count;
            m_UiRenderers.Remove(renderer);

            if (m_UiRenderers.Count == count)
            {
                return false;
            }
            return true;
        }
        public void ClearRenderers()
        {
            m_UiRenderers.Clear();
        }

        public override void Destroy()
        {
            base.Destroy();

            if (m_IsDisposed)
            {
                return;
            }

            m_UiRenderers.Clear();
            m_GraphicsDevice?.WaitForIdle();
            m_ImguiManager?.Dispose();
            m_CommandList?.Dispose();
            m_GraphicsDevice?.Dispose();
            if (m_Window != null)
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

        private void RenderMainMenuBar()
        {
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("Windows"))
                {
                    foreach (var renderer in m_UiRenderers)
                    {
                        if (ImGui.MenuItem(renderer.WindowName))
                        {
                            renderer.IsOpen = !renderer.IsOpen;
                        }
                    }
                    ImGui.EndMenu();
                }
            }
            ImGui.EndMainMenuBar();
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
