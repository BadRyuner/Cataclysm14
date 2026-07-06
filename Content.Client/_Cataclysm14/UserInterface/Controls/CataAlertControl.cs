using System.Linq;
using System.Numerics;
using Content.Client.Actions.UI;
using Content.Client.Cooldown;
using Content.Client.Stylesheets;
using Content.Shared.Alert;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Client._Cataclysm14.UserInterface.Controls;

public sealed class CataAlertControl : ContainerButton
{
    public RichTextLabel Label { get; }

    public AlertPrototype Alert { get; }

    public (TimeSpan Start, TimeSpan End)? Cooldown
    {
        get => _cooldown;
        set
        {
            _cooldown = value;
            if (SuppliedTooltip is ActionAlertTooltip actionAlertTooltip)
            {
                actionAlertTooltip.Cooldown = value;
            }
        }
    }

    public string LocalizedString
    {
        get
        {
            if (Alert.Name == _unLocString)
                return _locString;

            _unLocString = Alert.Name;
            _locString = $"\\[{Loc.GetString(_unLocString)}\\]";
            return _locString;
        }
    }

    private char _prevSymbol = 'Q';
    private bool _prevCooldown = false;

    private (TimeSpan Start, TimeSpan End)? _cooldown;

    private short? _severity;
    private readonly IGameTiming _gameTiming;
    private readonly IEntityManager _entityManager;

    private string _locString = null!;
    private string _unLocString = null!;

    public CataAlertControl(AlertPrototype alert, short? severity)
    {
        _gameTiming = IoCManager.Resolve<IGameTiming>();
        _entityManager = IoCManager.Resolve<IEntityManager>();
        TooltipSupplier = SupplyTooltip;
        Alert = alert;

        AddStyleClass(StyleClassButton);
        Label = new()
        {
            Text = LocalizedString,
        };
        AddChild(Label);
        Label.AddStyleClass(StyleClassButton);

        //Margin = new(0);

        HorizontalAlignment = HAlignment.Left;
        _severity = severity;
        StyleClasses.Add("StyleClassAlertButton");
    }

    private Control SupplyTooltip(Control? sender)
    {
        var msg = FormattedMessage.FromMarkupOrThrow(Loc.GetString(Alert.Name));
        var desc = FormattedMessage.FromMarkupOrThrow(Loc.GetString(Alert.Description));
        var tooltip = new ActionAlertTooltip(msg, desc) { Cooldown = Cooldown, StyleClasses = { StyleNano.StyleClassCataAlertPanel }, Margin = new(1)};
        var box = tooltip.Children[0];
        var name = box.Children[0];
        box.RemoveChild(name);
        name.Margin = new Thickness(5, 0,0,0);
        var otherChildren = box.Children.ToArray();
        foreach (var otherChild in otherChildren)
        {
            box.Children.Remove(otherChild);
        }
        box.AddChild(new PanelContainer() { StyleClasses = { StyleNano.StyleClassCataAlertPanel },
            Children =
            {
                new BoxContainer()
                {
                    StyleClasses = { StyleNano.StyleClassBoxOfTerminusLabels },
                    Children =
                    {
                        name
                    }
                }
            }});
        var otherThings = new BoxContainer()
        {
            Orientation = BoxContainer.LayoutOrientation.Vertical,
            StyleClasses = { StyleNano.StyleClassBoxOfTerminusLabels },
            Margin = new(5)
        };
        foreach (var otherChild in otherChildren)
        {
            otherThings.Children.Add(otherChild);
        }
        box.AddChild(otherThings);
        return tooltip;
    }

    protected override void FrameUpdate(FrameEventArgs args)
    {
        base.FrameUpdate(args);
        if (Cooldown.HasValue)
        {
            var time = FromTime(Cooldown.Value.Start, Cooldown.Value.End);
            var symbol = time switch
            {
                >= 0.98f => '⣿',
                >= 0.875f => '⣷',
                >= 0.75f => '⣶',
                >= 0.625f => '⣦',
                >= 0.5f => '⣤',
                >= 0.375f => '⣄',
                >= 0.25f => '⣀',
                >= 0.125f => '⡀',
                _ => ' ',
            };
            if (_prevSymbol != symbol)
            {
                _prevSymbol = symbol;
                Label.Text = $"{LocalizedString}({symbol})";
            }

            _prevCooldown = true;
        }

        if (!Cooldown.HasValue && _prevCooldown)
        {
            Label.Text = LocalizedString;
            _prevCooldown = false;
        }
    }

    public float FromTime(TimeSpan start, TimeSpan end) // from CooldownGraphic.FromTime
    {
        var duration = end - start;
        var curTime = _gameTiming.CurTime;
        var length = duration.TotalSeconds;
        var progress = (curTime - start).TotalSeconds / length;
        var ratio = (progress <= 1 ? (1 - progress) : (curTime - end).TotalSeconds * -5);

        return MathHelper.Clamp((float) ratio, -1, 1);
    }

    public void SetSeverity(short? severity)
    {
        if (_severity == severity)
            return;
        _severity = severity;

        //if (!_entityManager.TryGetComponent<SpriteComponent>(_spriteViewEntity, out var sprite))
        //    return;
        //var icon = Alert.GetIcon(_severity);
        //if (sprite.LayerMapTryGet(AlertVisualLayers.Base, out var layer))
        //    sprite.LayerSetSprite(layer, icon);
    }
}
