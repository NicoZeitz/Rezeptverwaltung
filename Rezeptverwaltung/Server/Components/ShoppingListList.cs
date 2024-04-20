using Core.Entities;
using Server.Resources;

namespace Server.Components;

public class ShoppingListList : TemplateComponent
{
    public IEnumerable<ShoppingList> ShoppingLists = Enumerable.Empty<ShoppingList>();

    public ShoppingListList(TemplateLoader templateLoader)
        : base(templateLoader)
    {
    }

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