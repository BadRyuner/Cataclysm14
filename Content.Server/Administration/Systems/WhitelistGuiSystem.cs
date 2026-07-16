// Cata14 add; handles client-to-server communication and whitelist management
// for the F7 whitelist admin interface
using Content.Server.Administration.Managers;
using Content.Server.Database;
using Content.Shared.Administration;
using Content.Shared.Administration.Whitelist;
using Robust.Server.Player;
using Robust.Shared.Network;
using Robust.Shared.Player;
using System.Threading.Tasks;
using System.Linq;

namespace Content.Server.Administration.Systems;

public sealed class WhitelistGuiSystem : EntitySystem
{
    [Dependency] private readonly IServerDbManager _db = default!;
    [Dependency] private readonly IAdminManager _adminManager = default!;
    [Dependency] private readonly IPlayerLocator _playerLocator = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeNetworkEvent<WhitelistGuiRequestEvent>(OnRequest);
        SubscribeNetworkEvent<WhitelistGuiAddEvent>(OnAdd);
        SubscribeNetworkEvent<WhitelistGuiRemoveEvent>(OnRemove);
    }

    private async void OnRequest(WhitelistGuiRequestEvent message, EntitySessionEventArgs args)
    {
        if (!HasAccess(args.SenderSession))
        {
            Send(args.SenderSession, [], "You do not have whitelist permission.", true);
            return;
        }

        await SendList(args.SenderSession);
    }

    private async void OnAdd(WhitelistGuiAddEvent message, EntitySessionEventArgs args)
    {
        if (!HasAccess(args.SenderSession))
        {
            Send(args.SenderSession, [], "You do not have whitelist permission.", true);
            return;
        }

        var input = message.UserNameOrId.Trim();
        NetUserId userId;
        string? userName = null;

        if (Guid.TryParse(input, out var guid))
        {
            userId = new NetUserId(guid);
            var record = await _db.GetPlayerRecordByUserId(userId);
            userName = record?.LastSeenUserName;
        }
        else
        {
            var located = await _playerLocator.LookupIdByNameAsync(input);
            if (located == null)
            {
                await SendList(args.SenderSession, $"No SS14 account or known player was found for '{input}'.", true);
                return;
            }

            userId = located.UserId;
            userName = located.Username;
        }

        if (await _db.GetWhitelistStatusAsync(userId))
        {
            await SendList(args.SenderSession, $"{userName ?? userId.ToString()} is already whitelisted.");
            return;
        }

        await _db.AddToWhitelistAsync(userId);
        await SendList(args.SenderSession, $"Added {userName ?? userId.ToString()} to the whitelist.");
    }

    private async void OnRemove(WhitelistGuiRemoveEvent message, EntitySessionEventArgs args)
    {
        if (!HasAccess(args.SenderSession))
        {
            Send(args.SenderSession, [], "You do not have whitelist permission.", true);
            return;
        }

        if (!await _db.GetWhitelistStatusAsync(message.UserId))
        {
            await SendList(args.SenderSession, "That account is no longer whitelisted.", true);
            return;
        }

        await _db.RemoveFromWhitelistAsync(message.UserId);
        await SendList(args.SenderSession, $"Removed {message.UserId} from the whitelist.");
    }

    private bool HasAccess(ICommonSession session)
    {
        return _adminManager.HasAdminFlag(session, AdminFlags.Whitelist, includeDeAdmin: true);
    }

    private async Task SendList(ICommonSession session, string? message = null, bool isError = false)
    {
        var entries = await _db.GetWhitelistEntriesAsync();
        Send(session,
            entries.Select(e => new WhitelistGuiEntry(e.UserId, e.UserName)).ToArray(),
            message,
            isError);
    }

    private void Send(ICommonSession session, WhitelistGuiEntry[] entries, string? message, bool isError)
    {
        RaiseNetworkEvent(new WhitelistGuiStateEvent(entries, message, isError), session);
    }
}
