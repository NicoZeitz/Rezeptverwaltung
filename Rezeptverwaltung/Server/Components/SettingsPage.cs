

using Server.Resources;

namespace Server.Components;

public class SettingsPage : ContainerComponent
{
    public SettingsPage(TemplateLoader templateLoader)
        : base(templateLoader)
    {
    }

    public override Task<string> RenderAsync()
    {
        throw new NotImplementedException();
    }
}
