// Cata14 add; manages whitelist's GUI network requests and responses,
// including loading, adding, and removing whitelist entries
using Content.Shared.Administration.Whitelist;
using Robust.Shared.Network;

namespace Content.Client.Administration.Systems;

public sealed class WhitelistGuiSystem : EntitySystem
{
    public event Action<IReadOnlyList<WhitelistGuiEntry>, string?, bool>? StateChanged;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeNetworkEvent<WhitelistGuiStateEvent>(OnState);
    }

    public void Refresh()
    {
        RaiseNetworkEvent(new WhitelistGuiRequestEvent());
    }

    public void Add(string userNameOrId)
    {
        RaiseNetworkEvent(new WhitelistGuiAddEvent(userNameOrId));
    }

    public void Remove(NetUserId userId)
    {
        RaiseNetworkEvent(new WhitelistGuiRemoveEvent(userId));
    }

    private void OnState(WhitelistGuiStateEvent message)
    {
        StateChanged?.Invoke(message.Entries, message.Message, message.IsError);
    }
}
