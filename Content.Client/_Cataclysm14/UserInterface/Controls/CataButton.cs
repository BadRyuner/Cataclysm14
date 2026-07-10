using Content.Client.Stylesheets;
using Robust.Client.UserInterface.Controls;

namespace Content.Client._Cataclysm14.UserInterface.Controls;

public sealed class CataButton : ContainerButton
{
    public RichTextLabel Label { get; }

    public CataButton()
    {
        AddStyleClass(StyleClassButton);
        AddChild(Label = new RichTextLabel
        {
            StyleClasses = { StyleClassButton },
        });

        StyleClasses.Add(StyleNano.StyleClassCataButton);
    }

    [ViewVariables]
    public string? Text { get => Label.Text; set => Label.Text = $"\\[{value}\\]"; }
}

