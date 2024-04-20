
using Server.Resources;
using System.Text;

namespace Server.Components;

public class ComponentSequence : ContainerComponent
{
    public ComponentSequence(TemplateLoader templateLoader) : base(templateLoader) { }

    public override async Task<string> RenderAsync()
    {
        return string.Join("", (await GetRenderedChildren()).Select(child => $"<span>{child}</span>"));
    }
}
