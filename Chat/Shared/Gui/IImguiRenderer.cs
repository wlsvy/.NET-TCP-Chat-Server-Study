namespace Shared.Gui
{
    public interface IImguiRenderer
    {
        string WindowName { get; }
        bool IsOpen { get; set; }
        void Render();
    }
}
