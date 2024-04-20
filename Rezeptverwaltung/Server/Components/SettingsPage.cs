

using Core.Entities;
using Server.Resources;

namespace Server.Components;

public class SettingsPage : ContainerComponent
{
    public Chef? CurrentChef { get; set; }

    public SettingsPage(TemplateLoader templateLoader)
        : base(templateLoader)
    {
    }

    public override async Task<string> RenderAsync()
    {
        return await templateLoader
            .LoadTemplate("SettingsPage.html")!
            .RenderAsync(new
            {
                Header = await GetRenderedSlottedChild("Header"),
                CurrentChef
            });
    }
}
