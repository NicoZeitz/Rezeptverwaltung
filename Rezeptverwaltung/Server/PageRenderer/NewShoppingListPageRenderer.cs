using Core.Entities;
using Core.ValueObjects;
using Server.Components;
using Server.Service;
using System.Net;

namespace Server.PageRenderer;

public class NewShoppingListPageRenderer
{
    private readonly ComponentProvider componentProvider;
    private readonly HTMLFileWriter htmlFileWriter;

    public NewShoppingListPageRenderer(
        ComponentProvider componentProvider,
        HTMLFileWriter htmlFileWriter)
    {
        this.componentProvider = componentProvider;
        this.htmlFileWriter = htmlFileWriter;
    }

    public async Task RenderPage(
        HttpListenerResponse response,
        HttpStatusCode httpStatus,
        IEnumerable<Recipe> recipes,
        Chef? currentChef,
        IEnumerable<ErrorMessage> errorMessages = default!)
    {
        errorMessages ??= [];

        var header = componentProvider.GetComponent<Header>();
        var newShoppingListPage = componentProvider.GetComponent<NewShoppingListPage>();

        header.CurrentChef = currentChef;
        newShoppingListPage.SlottedChildren[NewShoppingListPage.HEADER_SLOT] = header;
        newShoppingListPage.Children = errorMessages.Select(errorMessage => new DisplayableComponent(errorMessage));
        newShoppingListPage.Recipes = recipes;

        var htmlString = await newShoppingListPage.RenderAsync();
        htmlFileWriter.WriteHtmlFile(response, htmlString, httpStatus);
    }

}