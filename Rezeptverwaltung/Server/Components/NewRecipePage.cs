

using Core.Entities;
using Core.ValueObjects;
using Server.Resources;

namespace Server.Components;

public class NewRecipePage : ContainerComponent
{
    public IEnumerable<Tag> Tags { get; set; } = [];
    public IEnumerable<Ingredient> Ingredients { get; set; } = [];
    public Recipe? Recipe { get; set; }
    public IEnumerable<Text> Units { get; set; } = [];

    public NewRecipePage(TemplateLoader templateLoader)
        : base(templateLoader)
    {
    }

    public override async Task<string> RenderAsync()
    {
        return await templateLoader
            .LoadTemplate("NewRecipePage.html")!
            .RenderAsync(new
            {
                Recipe,
                Tags,
                Units,
                Ingredients,
                Header = await GetRenderedSlottedChild("Header"),
                Children = await GetRenderedChildren()
            });
    }
}
