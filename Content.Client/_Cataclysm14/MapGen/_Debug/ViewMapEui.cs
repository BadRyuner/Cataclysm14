using Content.Client.Eui;
using Content.Shared.Eui;

namespace Content.Client._Cataclysm14.MapGen._Debug;

public sealed class ViewMapEui : BaseEui
{
    private readonly ViewMapWindow _window;

    public ViewMapEui()
    {
        _window = new();
        _window.OnClose += () => SendMessage(new CloseEuiMessage());
    }

    public override void Opened()
    {
        _window.OpenCentered();
    }

    public override void Closed()
    {
        _window.Close();
    }
}
