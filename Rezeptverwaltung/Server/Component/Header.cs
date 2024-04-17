using Core.Entities;
using Server.Resources;

namespace Server.Component;

public class Header
{
    private readonly TemplateLoader templateLoader;

    public Header(ResourceLoader.ResourceLoader resourceLoader)
    {
        templateLoader = new TemplateLoader(resourceLoader);
    }

    public ValueTask<string> RenderAsync(Chef? currentChef)
    {
        return templateLoader
            .LoadTemplate("header.html")!
            .RenderAsync(new
            {
                Chef = currentChef
            });
    }
}