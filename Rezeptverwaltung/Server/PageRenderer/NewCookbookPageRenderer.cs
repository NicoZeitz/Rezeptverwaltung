using Core.Entities;
using Core.Services;
using Core.Services.Serialization;
using Core.ValueObjects;
using Server.Components;
using Server.Service;
using System.Net;

namespace Server.PageRenderer;

public class NewCookbookPageRenderer
{
    private readonly ComponentProvider componentProvider;
    private readonly HTMLFileWriter htmlFileWriter;
    private readonly MeasurementUnitSerializationManager measurementUnitSerializationManager;
    private readonly ShowRecipes showRecipes;

    public NewCookbookPageRenderer(
        ComponentProvider componentProvider,
        HTMLFileWriter htmlFileWriter,
        MeasurementUnitSerializationManager measurementUnitSerializationManager,
        ShowRecipes showRecipes)
    {
        this.componentProvider = componentProvider;
        this.htmlFileWriter = htmlFileWriter;
        this.measurementUnitSerializationManager = measurementUnitSerializationManager;
        this.showRecipes = showRecipes;
    }

    public async Task RenderPage(
        HttpListenerResponse response,
        HttpStatusCode httpStatus,
        IEnumerable<Recipe> recipes,
        Chef? currentChef,
        IEnumerable<ErrorMessage> errorMessages = default!)
    {
        errorMessages ??= [];

        var tags = showRecipes.ShowAllTags(currentChef);
        var ingredients = showRecipes.ShowAllIngredients(currentChef);
        var units = measurementUnitSerializationManager.UnitsToDeserialize.Select(unit => new Text(unit));

        var header = componentProvider.GetComponent<Header>();
        var newCookbookPage = componentProvider.GetComponent<NewCookbookPage>();

        header.CurrentChef = currentChef;
        newCookbookPage.SlottedChildren[NewRecipePage.HEADER_SLOT] = header;
        newCookbookPage.Children = errorMessages.Select(errorMessage => new DisplayableComponent(errorMessage));
        newCookbookPage.Recipes = recipes;

        var htmlString = await newCookbookPage.RenderAsync();
        htmlFileWriter.WriteHtmlFile(response, htmlString, httpStatus);
    }

}