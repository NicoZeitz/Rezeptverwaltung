using Core.Entities;
using Server.Resources;

namespace Server.Components;

public class CookbookList : Component
{
    public IEnumerable<Cookbook> Cookbooks { get; set; } = [];

    private readonly TemplateLoader templateLoader;

    public CookbookList(TemplateLoader templateLoader)
    {
        this.templateLoader = templateLoader;
    }

    public Task<string> RenderAsync()
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