using Core.Entities;
using Core.Services;
using Core.Services.Serialization;
using Core.ValueObjects;
using Server.Components;
using Server.Service;
using System.Net;

namespace Server.PageRenderer;

public class RecipeEditPageRenderer
{
    private readonly ComponentProvider componentProvider;
    private readonly HTMLFileWriter htmlFileWriter;
    private readonly MeasurementUnitSerializationManager measurementUnitSerializationManager;
    private readonly ShowRecipes showRecipes;

    public RecipeEditPageRenderer(
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
        Recipe? recipe,
        Chef? currentChef,
        IEnumerable<ErrorMessage> errorMessages = default!)
    {
        errorMessages ??= [];

        var tags = showRecipes.ShowAllTags(currentChef);
        var ingredients = showRecipes.ShowAllIngredients(currentChef);
        var units = measurementUnitSerializationManager.UnitsToDeserialize.Select(unit => new Text(unit));

        var header = componentProvider.GetComponent<Header>();
        var newRecipePage = componentProvider.GetComponent<NewRecipePage>();

        header.CurrentChef = currentChef;
        newRecipePage.SlottedChildren[NewRecipePage.HEADER_SLOT] = header;
        newRecipePage.Children = errorMessages.Select(errorMessage => new DisplayableComponent(errorMessage));
        newRecipePage.Recipe = recipe;
        newRecipePage.Tags = tags;
        newRecipePage.Ingredients = ingredients;
        newRecipePage.Units = units;

        var htmlString = await newRecipePage.RenderAsync();
        htmlFileWriter.WriteHtmlFile(response, htmlString, httpStatus);
    }

}