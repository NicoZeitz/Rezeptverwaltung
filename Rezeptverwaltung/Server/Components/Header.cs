using Core.Entities;
using Server.Resources;

namespace Server.Components;

public class Header : TemplateComponent
{
    public Chef? CurrentChef { get; set; }

    public Header(TemplateLoader templateLoader)
        : base(templateLoader)
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