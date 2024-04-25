using Core.Services;
using Core.ValueObjects;
using Server.Components;
using Server.PageRenderer;
using Server.Service;
using Server.Session;
using System.Net;
using System.Text.RegularExpressions;

namespace Server.RequestHandler;

public partial class ChefDetailRequestHandler : HTMLRequestHandler
{
    private readonly ComponentProvider componentProvider;
    private readonly ShowChefs showChefs;
    private readonly ShowCookbooks showCookbooks;
    private readonly ShowRecipes showRecipes;
    private readonly ShowShoppingLists showShoppingLists;

    [GeneratedRegex("^/chef/(?<chef_username>[A-Z0-9_ ]+)/?$", RegexOptions.NonBacktracking | RegexOptions.IgnoreCase, matchTimeoutMilliseconds: 1000)]
    private static partial Regex chefDetailUrlPathRegex();

    public ChefDetailRequestHandler(
        ComponentProvider componentProvider,
        HTMLFileWriter htmlFileWriter,
        NotFoundPageRenderer notFoundPageRenderer,
        SessionService sessionService,
        ShowChefs showChefs,
        ShowCookbooks showCookbooks,
        ShowRecipes showRecipes,
        ShowShoppingLists showShoppingLists)
        : base(htmlFileWriter, notFoundPageRenderer, sessionService)
    {
        this.componentProvider = componentProvider;
        this.showChefs = showChefs;
        this.showCookbooks = showCookbooks;
        this.showRecipes = showRecipes;
        this.showShoppingLists = showShoppingLists;
    }

    public override bool CanHandle(HttpListenerRequest request) =>
        request.HttpMethod == HttpMethod.Get.Method
        && chefDetailUrlPathRegex().IsMatch(request.Url?.AbsolutePath ?? "");

    public override Task<string> HandleHtmlFileRequest(HttpListenerRequest request)
    {
        var pageData = ExtractDataFromRequest(request);
        if (pageData.Chef is null)
        {
            return notFoundPageRenderer.RenderPage(pageData.CurrentChef);
        }

        var chefDetailPage = SetupUiComponents(pageData);
        return chefDetailPage.RenderAsync();
    }

    private Username GetChefUsernameFromRequest(HttpListenerRequest request)
    {
        return new Username(chefDetailUrlPathRegex().Match(request.Url!.AbsolutePath).Groups["chef_username"].Value);
    }

    private ChefDetailPageData ExtractDataFromRequest(HttpListenerRequest request)
    {
        var currentChef = sessionService.GetCurrentChef(request);
        var username = GetChefUsernameFromRequest(request);
        var chef = showChefs.ShowSingleChef(username);
        var recipes = chef is null ? [] : showRecipes.ShowRecipesForChef(chef, currentChef);
        var cookbooks = chef is null ? [] : showCookbooks.ShowCookbooksForChef(chef, currentChef);
        var shoppingLists = chef is null ? [] : showShoppingLists.ShowShoppingListsForChef(chef, currentChef);

        return new ChefDetailPageData(
            currentChef,
            chef,
            recipes,
            cookbooks,
            shoppingLists
        );
    }

    private Component SetupUiComponents(ChefDetailPageData pageData)
    {
        var header = componentProvider.GetComponent<Header>();
        var recipeList = componentProvider.GetComponent<RecipeList>();
        var cookbookList = componentProvider.GetComponent<CookbookList>();
        var shoppingListList = componentProvider.GetComponent<ShoppingListList>();
        var chefDetailPage = componentProvider.GetComponent<ChefDetailPage>();

        header.CurrentChef = pageData.CurrentChef;
        recipeList.Recipes = pageData.Recipes;
        cookbookList.Cookbooks = pageData.Cookbooks;
        shoppingListList.ShoppingLists = pageData.ShoppingLists;
        chefDetailPage.Chef = pageData.Chef;

        chefDetailPage.SlottedChildren[ChefDetailPage.HEADER_SLOT] = header;
        chefDetailPage.SlottedChildren[ChefDetailPage.RECIPES_SLOT] = recipeList;
        chefDetailPage.SlottedChildren[ChefDetailPage.COOKBOOKS_SLOT] = cookbookList;
        chefDetailPage.SlottedChildren[ChefDetailPage.SHOPPING_LISTS_SLOT] = shoppingListList;

        return chefDetailPage;
    }

    private record struct ChefDetailPageData(
        Core.Entities.Chef? CurrentChef,
        Core.Entities.Chef? Chef,
        IEnumerable<Core.Entities.Recipe> Recipes,
        IEnumerable<Core.Entities.Cookbook> Cookbooks,
        IEnumerable<Core.Entities.ShoppingList> ShoppingLists);
}