using Robust.Shared;
using Robust.Shared.Configuration;

namespace Content.Shared._Cataclysm14.CCVar;

[CVarDefs]
// ReSharper disable once IdentifierTypo
public sealed class CataCCVars
{
    /// <summary>
    /// Intervals at which wild food respawns
    /// </summary>
    public static readonly CVarDef<float> ForageRespawnCooldown =
        CVarDef.Create("cata.forage_respawn_cooldown", 1200f, CVar.SERVERONLY); // 1200f = 1200 sec = 20 min
}
