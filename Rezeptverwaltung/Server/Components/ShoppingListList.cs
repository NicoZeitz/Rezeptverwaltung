using Core.Entities;
using Server.Resources;

namespace Server.Components;

public class ShoppingListList(TemplateLoader templateLoader) : TemplateComponent(templateLoader)
{
    public IEnumerable<ShoppingList> ShoppingLists = [];

    public override Task<string> RenderAsync()
    {
        return templateLoader
            .LoadTemplate("ShoppingListList.html")!
            .RenderAsync(new
            {
                ShoppingLists
            })
            .AsTask();
    }
}