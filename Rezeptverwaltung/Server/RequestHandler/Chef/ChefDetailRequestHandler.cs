using Core.Services;
using Core.ValueObjects;
using Server.Components;
using Server.Service;
using Server.Session;
using System.Net;
using System.Text.RegularExpressions;

namespace Server.RequestHandler;

public partial class ChefDetailRequestHandler : HTMLRequestHandler
{
    private readonly ChefDetailPage chefDetailPage;
    private readonly CookbookList cookbookList;
    private readonly Header header;
    private readonly NotFoundPageRenderer notFoundPageRenderer;
    private readonly RecipeList recipeList;
    private readonly SessionService sessionService;
    private readonly ShoppingListList shoppingListList;
    private readonly ShowChefs showChefs;
    private readonly ShowCookbooks showCookbooks;
    private readonly ShowRecipes showRecipes;
    private readonly ShowShoppingLists showShoppingLists;

    [GeneratedRegex("/chef/(?<chef_username>[A-Z0-9_ ]+)", RegexOptions.NonBacktracking | RegexOptions.IgnoreCase, matchTimeoutMilliseconds: 1000)]
    private static partial Regex chefDetailUrlPathRegex();

    public ChefDetailRequestHandler(
        ChefDetailPage chefDetailPage,
        CookbookList cookbookList,
        Header header,
        HTMLFileWriter htmlFileWriter,
        NotFoundPageRenderer notFoundPageRenderer,
        RecipeList recipeList,
        SessionService sessionService,
        ShoppingListList shoppingListList,
        ShowChefs showChefs,
        ShowCookbooks showCookbooks,
        ShowRecipes showRecipes,
        ShowShoppingLists showShoppingLists)
        : base(htmlFileWriter)
    {
        this.chefDetailPage = chefDetailPage;
        this.cookbookList = cookbookList;
        this.header = header;
        this.notFoundPageRenderer = notFoundPageRenderer;
        this.recipeList = recipeList;
        this.sessionService = sessionService;
        this.shoppingListList = shoppingListList;
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

        SetDataOnComponents(pageData);
        SetSlottedComponents();
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
        var recipes = chef is null ? Enumerable.Empty<Core.Entities.Recipe>() : showRecipes.ShowRecipesForChef(chef, currentChef);
        var cookbooks = chef is null ? Enumerable.Empty<Core.Entities.Cookbook>() : showCookbooks.ShowCookbooksForChef(chef, currentChef);
        var shoppingLists = chef is null ? Enumerable.Empty<Core.Entities.ShoppingList>() : showShoppingLists.ShowShoppingListsForChef(chef, currentChef);

        return new ChefDetailPageData(
            currentChef,
            chef,
            recipes,
            cookbooks,
            shoppingLists
        );
    }

    private void SetDataOnComponents(ChefDetailPageData pageData)
    {
        header.CurrentChef = pageData.CurrentChef;
        recipeList.Recipes = pageData.Recipes;
        cookbookList.Cookbooks = pageData.Cookbooks;
        shoppingListList.ShoppingLists = pageData.ShoppingLists;
        chefDetailPage.Chef = pageData.Chef;
    }

    private void SetSlottedComponents()
    {
        chefDetailPage.SlottedChildren["Recipes"] = recipeList;
        chefDetailPage.SlottedChildren["Cookbooks"] = cookbookList;
        chefDetailPage.SlottedChildren["ShoppingLists"] = shoppingListList;
        chefDetailPage.SlottedChildren["Header"] = header;
    }

    private record struct ChefDetailPageData(
        Core.Entities.Chef? CurrentChef,
        Core.Entities.Chef? Chef,
        IEnumerable<Core.Entities.Recipe> Recipes,
        IEnumerable<Core.Entities.Cookbook> Cookbooks,
        IEnumerable<Core.Entities.ShoppingList> ShoppingLists);
}