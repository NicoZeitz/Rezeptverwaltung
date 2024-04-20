using Core.ValueObjects;
using Server.Resources;

namespace Server.Components;

public class TagPage : ContainerComponent
{
    public Tag? Tag { get; set; }

    public TagPage(TemplateLoader templateLoader)
        : base(templateLoader)
    {
    }

    public override async Task<string> RenderAsync()
    {
        return await templateLoader
            .LoadTemplate("TagPage.html")!
            .RenderAsync(new
            {
                Tag,
                Children = await GetRenderedChildren(),
                Header = await GetRenderedSlottedChild("Header")
            });
    }
}