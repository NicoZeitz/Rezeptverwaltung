using Core.Entities;
using Core.ValueObjects;
using Server.Components;
using Server.Resources;
using Server.Service;
using System.Net;

namespace Server.RequestHandler;

public class NewRecipePageRenderer
{
    private readonly ComponentProvider componentProvider;
    private readonly HTMLFileWriter htmlFileWriter;
    private readonly ImageUrlService imageUrlService;

    public NewRecipePageRenderer(
        ComponentProvider componentProvider,
        HTMLFileWriter htmlFileWriter,
        ImageUrlService imageUrlService)
    {
        this.componentProvider = componentProvider;
        this.htmlFileWriter = htmlFileWriter;
        this.imageUrlService = imageUrlService;
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
        newRecipePage.SlottedChildren["Header"] = header;
        newRecipePage.Children = errorMessages.Select(errorMessage => new DisplayableComponent(errorMessage));
        newRecipePage.Recipe = recipe;
        newRecipePage.Tags = tags;
        newRecipePage.Ingredients = ingredients;
        newRecipePage.Units = new[] { new Text("g"), new Text("kg"), new Text("ml"), new Text("l") }; // TODO:

        var htmlString = await newRecipePage.RenderAsync();
        htmlFileWriter.WriteHtmlFile(response, htmlString, httpStatus);
    }

}