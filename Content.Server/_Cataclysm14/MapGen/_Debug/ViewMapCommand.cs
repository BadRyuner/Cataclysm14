using Content.Server.Administration;
using Content.Server.Administration.UI;
using Content.Server.EUI;
using Content.Shared.Administration;
using Robust.Shared.Console;

namespace Content.Server._Cataclysm14.MapGen._Debug;

[AdminCommand(AdminFlags.Debug)]
public sealed class ViewMapCommand : IConsoleCommand
{
    public string Command => "viewmapgen";

    public string Description => "Opens debug map";

    public string Help => $"{Command}";

    public void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        var player = shell.Player;
        if (player == null)
        {
            shell.WriteLine("This does not work from the server console.");
            return;
        }

        var eui = IoCManager.Resolve<EuiManager>();
        var ui = new ViewMapEui();
        eui.OpenEui(ui, player);
    }
}
