

using Core.Entities;
using Core.ValueObjects;
using Server.Resources;

namespace Server.Components;

public class ShoppingListDetailPage(TemplateLoader templateLoader) : ContainerComponent(templateLoader)
{
    public const string HEADER_SLOT = "Header";

    public IEnumerable<WeightedIngredient> Ingredients { get; set; } = [];
    public ShoppingList? ShoppingList { get; set; }
    public Chef? CurrentChef { get; set; }

    public override async Task<string> RenderAsync()
    {
        return await templateLoader
            .LoadTemplate("ShoppingListDetailPage.html")
            .RenderAsync(new
            {
                ShoppingList,
                CurrentChef,
                Ingredients,
                Header = await GetRenderedSlottedChild(HEADER_SLOT),
                Children = await GetRenderedChildren()
            })
            .AsTask();
    }
}
