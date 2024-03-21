using Core.Entities;
using Scriban;
using Server.ResourceLoader;

namespace Server.Component;

public class Header
{
    private readonly IResourceLoader resourceLoader;

    public Header(IResourceLoader resourceLoader)
    {
        this.resourceLoader = resourceLoader;
    }

    public ValueTask<string> RenderAsync(Chef? currentChef)
    {
        using var headerStream = resourceLoader.LoadResource("header.html")!;
        var headerContent = new StreamReader(headerStream).ReadToEnd();
        var headerTemplate = Template.Parse(headerContent);



        return headerTemplate.RenderAsync(new { Chef = currentChef });
    }
}