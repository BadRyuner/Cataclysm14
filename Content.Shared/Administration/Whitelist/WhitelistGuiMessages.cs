// Cata14 add; requests that an account be removed from the connection whitelist
using Content.Shared.Eui;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration.Whitelist;

[Serializable, NetSerializable]
public sealed class WhitelistGuiRequestEvent : EntityEventArgs;

[Serializable, NetSerializable]
public sealed class WhitelistGuiAddEvent(string userNameOrId) : EntityEventArgs
{
    public readonly string UserNameOrId = userNameOrId;
}

[Serializable, NetSerializable]
public sealed class WhitelistGuiRemoveEvent(NetUserId userId) : EntityEventArgs
{
    public readonly NetUserId UserId = userId;
}

[Serializable, NetSerializable]
public sealed class WhitelistGuiStateEvent(
    WhitelistGuiEntry[] entries,
    string? message = null,
    bool isError = false) : EntityEventArgs
{
    public readonly WhitelistGuiEntry[] Entries = entries;
    public readonly string? Message = message;
    public readonly bool IsError = isError;
}

[Serializable, NetSerializable]
public readonly record struct WhitelistGuiEntry(NetUserId UserId, string? UserName);
