using Core.Entities;
using Core.ValueObjects;
using Server.Components;
using Server.Service;
using System.Net;

namespace Server.PageRenderer;

public class RecipeEditPageRenderer
{
    private readonly ComponentProvider componentProvider;
    private readonly HTMLFileWriter htmlFileWriter;

    public RecipeEditPageRenderer(
        ComponentProvider componentProvider,
        HTMLFileWriter htmlFileWriter)
    {
        this.componentProvider = componentProvider;
        this.htmlFileWriter = htmlFileWriter;
    }

    public async Task RenderPage(
        HttpListenerResponse response,
        HttpStatusCode httpStatus,
        Recipe? recipe,
        Chef? currentChef,
        IEnumerable<Tag> tags,
        IEnumerable<Ingredient> ingredients,
        IEnumerable<ErrorMessage> errorMessages = default!
    )
    {
        errorMessages ??= [];

        var header = componentProvider.GetComponent<Header>();
        var newRecipePage = componentProvider.GetComponent<NewRecipePage>();

        header.CurrentChef = currentChef;
        newRecipePage.SlottedChildren[NewRecipePage.HEADER_SLOT] = header;
        newRecipePage.Children = errorMessages.Select(errorMessage => new DisplayableComponent(errorMessage));
        newRecipePage.Recipe = recipe;
        newRecipePage.Tags = tags;
        newRecipePage.Ingredients = ingredients;
        newRecipePage.Units = [new Text("g"), new Text("kg"), new Text("ml"), new Text("l")]; // TODO:

        var htmlString = await newRecipePage.RenderAsync();
        htmlFileWriter.WriteHtmlFile(response, htmlString, httpStatus);
    }

}