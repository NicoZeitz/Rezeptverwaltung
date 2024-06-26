using Core.Entities;
using Core.ValueObjects;
using Server.Components;
using Server.Service;
using System.Net;

namespace Server.PageRenderer;

public class NewCookbookPageRenderer
{
    private readonly ComponentProvider componentProvider;
    private readonly HTMLFileWriter htmlFileWriter;

    public NewCookbookPageRenderer(
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
        var newCookbookPage = componentProvider.GetComponent<NewCookbookPage>();

        header.CurrentChef = currentChef;
        newCookbookPage.SlottedChildren[NewCookbookPage.HEADER_SLOT] = header;
        newCookbookPage.Children = errorMessages.Select(errorMessage => new DisplayableComponent(errorMessage));
        newCookbookPage.Recipes = recipes;

        var htmlString = await newCookbookPage.RenderAsync();
        htmlFileWriter.WriteHtmlFile(response, htmlString, httpStatus);
    }

}