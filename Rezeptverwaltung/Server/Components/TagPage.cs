using Core.ValueObjects;
using Server.Resources;

namespace Server.Components;

public class TagPage(TemplateLoader templateLoader) : ContainerComponent(templateLoader)
{
    public const string HEADER_SLOT = "Header";

    public Tag? Tag { get; set; }

    public override async Task<string> RenderAsync()
    {
        return await templateLoader
            .LoadTemplate("TagPage.html")!
            .RenderAsync(new
            {
                Tag,
                Children = await GetRenderedChildren(),
                Header = await GetRenderedSlottedChild(HEADER_SLOT)
            });
    }
}