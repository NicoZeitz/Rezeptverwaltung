using Core.Entities;
using Server.Resources;

namespace Server.Components;

public class CookbookList(TemplateLoader templateLoader) : TemplateComponent(templateLoader)
{
    public IEnumerable<Cookbook> Cookbooks { get; set; } = [];

    public override Task<string> RenderAsync()
    {
        return templateLoader
            .LoadTemplate("CookbookList.html")!
            .RenderAsync(new
            {
                Cookbooks
            })
            .AsTask();
    }
}