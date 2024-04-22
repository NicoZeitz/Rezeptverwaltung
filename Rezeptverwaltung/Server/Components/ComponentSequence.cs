
using Server.Resources;

namespace Server.Components;

public class ComponentSequence(TemplateLoader templateLoader) : ContainerComponent(templateLoader)
{
    public override async Task<string> RenderAsync()
    {
        return string.Join("", (await GetRenderedChildren()).Select(child => $"<span>{child}</span>"));
    }
}
