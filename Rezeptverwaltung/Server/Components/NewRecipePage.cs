using Core.Entities;
using Core.ValueObjects;
using Server.Resources;

namespace Server.Components;

public class NewRecipePage(TemplateLoader templateLoader) : ContainerComponent(templateLoader)
{
    public const string HEADER_SLOT = "Header";

    public IEnumerable<Tag> Tags { get; set; } = [];
    public IEnumerable<Ingredient> Ingredients { get; set; } = [];
    public Recipe? Recipe { get; set; }
    public IEnumerable<Text> Units { get; set; } = [];

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
                Header = await GetRenderedSlottedChild(HEADER_SLOT),
                Children = await GetRenderedChildren()
            });
    }
}
