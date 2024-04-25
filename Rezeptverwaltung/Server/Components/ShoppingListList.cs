using Core.Entities;
using Server.Resources;

namespace Server.Components;

public class ShoppingListList : Component
{
    private readonly TemplateLoader templateLoader;

    public ShoppingListList(TemplateLoader templateLoader)
    {
        this.templateLoader = templateLoader;
    }

    public IEnumerable<ShoppingList> ShoppingLists = [];

    public Task<string> RenderAsync()
    {
        if (!ShoppingLists.Any())
        {
            return Task.FromResult("");
        }

        return templateLoader
            .LoadTemplate("ShoppingListList.html")!
            .RenderAsync(new
            {
                ShoppingLists
            })
            .AsTask();
    }
}