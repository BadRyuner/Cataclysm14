using Content.Client._Cataclysm14.Crafting;
using Content.Client.Gameplay;
using Content.Client.UserInterface.Controls;
using Content.Client.UserInterface.Systems.MenuBar.Widgets;
using Content.Shared.Input;
using JetBrains.Annotations;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Input.Binding;
using Robust.Shared.Utility;

namespace Content.Client._Cataclysm14.UserInterface.Systems.Crafting;

[UsedImplicitly]
public sealed class CraftingUIController : UIController, IOnStateEntered<GameplayState>, IOnStateExited<GameplayState>
{
    private CraftingWindow? _window;
    private MenuButton? CraftingButton => UIManager.GetActiveUIWidgetOrNull<GameTopMenuBar>()?.CataCraftingButton;

    public void OnStateEntered(GameplayState state)
    {
        DebugTools.Assert(_window == null);

        _window = UIManager.CreateWindow<CraftingWindow>();
        LayoutContainer.SetAnchorPreset(_window, LayoutContainer.LayoutPreset.CenterTop);

        _window.OnClose += () =>
        {
            if (CraftingButton != null)
                CraftingButton.Pressed = false;
        };
        _window.OnOpen += () =>
        {
            if (CraftingButton != null)
                CraftingButton.Pressed = true;
        };

        CommandBinds.Builder.Bind(ContentKeyFunctions.OpenCataCraftingMenu,
            InputCmdHandler.FromDelegate(_ => ToggleWindow())).Register<CraftingUIController>();
    }

    public void OnStateExited(GameplayState state)
    {
        if (_window != null)
        {
            _window.Dispose();
            _window = null;
        }

        CommandBinds.Unregister<CraftingUIController>();
    }

    public void UnloadButton()
    {
        if (CraftingButton == null)
            return;

        CraftingButton.OnPressed -= CraftingButtonPressed;
    }

    public void LoadButton()
    {
        if (CraftingButton == null)
            return;

        CraftingButton.OnPressed += CraftingButtonPressed;
    }

    private void CraftingButtonPressed(BaseButton.ButtonEventArgs args)
    {
        ToggleWindow();
    }

    private void ToggleWindow()
    {
        if (_window == null)
            return;

        if (CraftingButton != null)
            CraftingButton.SetClickPressed(!_window.IsOpen);

        if (_window.IsOpen)
            _window.Close();
        else
            _window.Open();
    }
}
