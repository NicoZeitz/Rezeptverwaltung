using Core.Entities;
using Server.Resources;

namespace Server.Components;

public class CookbookList : TemplateComponent
{
    public IEnumerable<Cookbook> Cookbooks { get; set; } = [];

    public CookbookList(TemplateLoader templateLoader)
        : base(templateLoader)
    { }

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