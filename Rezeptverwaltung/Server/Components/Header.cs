using Core.Entities;

namespace Server.Components;

public class Header : TemplateComponent
{
    public Chef? CurrentChef { get; set; }

    public Header(ResourceLoader.ResourceLoader resourceLoader)
        : base(resourceLoader)
    {
    }

    public override Task<string> RenderAsync()
    {
        return templateLoader
            .LoadTemplate("Header.html")!
            .RenderAsync(new
            {
                Chef = CurrentChef
            })
            .AsTask();
    }
}