using Core.Entities;
using Server.Resources;

namespace Server.Components;

public class CookbookList(TemplateLoader templateLoader) : TemplateComponent(templateLoader)
{
    public IEnumerable<Cookbook> Cookbooks { get; set; } = [];

    public override Task<string> RenderAsync()
    {
        if (!Cookbooks.Any())
        {
            return Task.FromResult("");
        }

        return templateLoader
            .LoadTemplate("CookbookList.html")!
            .RenderAsync(new
            {
                Cookbooks
            })
            .AsTask();
    }
}