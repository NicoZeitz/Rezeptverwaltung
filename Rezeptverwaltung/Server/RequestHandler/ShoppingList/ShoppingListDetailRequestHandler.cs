using Core.Entities;
using Core.Services;
using Core.ValueObjects;
using Core.ValueObjects.MeasurementUnits;
using Server.Components;
using Server.PageRenderer;
using Server.Service;
using Server.Session;
using System.Net;
using System.Text.RegularExpressions;

namespace Server.RequestHandler;

public partial class ShoppingListDetailRequestHandler : HTMLRequestHandler
{
    [GeneratedRegex("^/shopping-list/(?<shopping_list_id>[a-fA-F0-9]{8}-[a-fA-F0-9]{4}-4[a-fA-F0-9]{3}-[89AB][a-fA-F0-9]{3}-[a-fA-F0-9]{12})/?$", RegexOptions.NonBacktracking | RegexOptions.IgnoreCase, matchTimeoutMilliseconds: 1000)]
    private static partial Regex shoppingListUrlPathRegex();

    private readonly ComponentProvider componentProvider;
    private readonly ShowRecipes showRecipes;
    private readonly ShowPortionedRecipesFromShoppingList showPortionedRecipesFromShoppingList;
    private readonly ShowWeightedIngredientsForShoppingList showWeightedIngredientsForShoppingList;
    private readonly ShowShoppingLists showShoppingLists;

    public ShoppingListDetailRequestHandler(
        HTMLFileWriter htmlFileWriter,
        ComponentProvider componentProvider,
        NotFoundPageRenderer notFoundPageRenderer,
        ShowRecipes showRecipes,
        ShowPortionedRecipesFromShoppingList showPortionedRecipesFromShoppingList,
        ShowShoppingLists showShoppingLists,
        ShowWeightedIngredientsForShoppingList showWeightedIngredientsForShoppingList,
        SessionService sessionService)
        : base(htmlFileWriter, notFoundPageRenderer, sessionService)
    {
        this.componentProvider = componentProvider;
        this.showRecipes = showRecipes;
        this.showPortionedRecipesFromShoppingList = showPortionedRecipesFromShoppingList;
        this.showShoppingLists = showShoppingLists;
        this.showWeightedIngredientsForShoppingList = showWeightedIngredientsForShoppingList;
    }

    public override bool CanHandle(HttpListenerRequest request) =>
        request.HttpMethod == HttpMethod.Get.Method
        && shoppingListUrlPathRegex().IsMatch(request.Url?.AbsolutePath ?? "");

    public override Task<string> HandleHtmlFileRequest(HttpListenerRequest request)
    {
        var pageData = ExtractDataFromRequest(request);
        if (pageData.ShoppingList is null)
        {
            return ReturnNotFound();
        }

        var header = componentProvider.GetComponent<Header>();
        var recipeList = componentProvider.GetComponent<RecipeList>();
        var shoppingListDetailPage = componentProvider.GetComponent<ShoppingListDetailPage>();

        recipeList.Recipes = pageData.Recipes
            .Select(recipe => recipe.Item2)
            .Distinct();
        header.CurrentChef = pageData.CurrentChef;
        shoppingListDetailPage.ShoppingList = pageData.ShoppingList;
        shoppingListDetailPage.Ingredients = pageData.WeightedIngredients;
        shoppingListDetailPage.CurrentChef = pageData.CurrentChef;
        shoppingListDetailPage.SlottedChildren[RecipeDetailPage.HEADER_SLOT] = header;
        shoppingListDetailPage.Children = [recipeList];
        return shoppingListDetailPage.RenderAsync();
    }

    private ShoppingListDetailPageData ExtractDataFromRequest(HttpListenerRequest request)
    {
        var currentChef = sessionService.GetCurrentChef(request);
        var shoppingListId = GetShoppingListIdFromRequest(request);
        var shoppingList = showShoppingLists.ShowSingleShoppingList(shoppingListId, currentChef);
        var recipes = shoppingList is null ? [] : showPortionedRecipesFromShoppingList.ShowRecipes(shoppingList, currentChef);
        var weightedIngredients = shoppingList is null ? [] : showWeightedIngredientsForShoppingList.ShowIngredients(shoppingList, currentChef);

        return new ShoppingListDetailPageData(
            currentChef,
            shoppingList,
            recipes,
            weightedIngredients
        );
    }

    private Identifier GetShoppingListIdFromRequest(HttpListenerRequest request)
         => Identifier.Parse(shoppingListUrlPathRegex().Match(request.Url!.AbsolutePath).Groups["shopping_list_id"].Value)!.Value;

    private record struct ShoppingListDetailPageData(
        Chef? CurrentChef,
        ShoppingList? ShoppingList,
        IEnumerable<(Portion, Recipe)> Recipes,
        IEnumerable<WeightedIngredient> WeightedIngredients
    );
}