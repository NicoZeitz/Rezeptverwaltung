using Core.Entities;
using Core.Services;
using Core.ValueObjects;
using Server.Components;
using Server.PageRenderer;
using Server.Service;
using Server.Session;
using System.Net;
using System.Text.RegularExpressions;

namespace Server.RequestHandler;

public partial class CookbookDetailRequestHandler : HTMLRequestHandler
{
    [GeneratedRegex("^/cookbook/(?<cookbook_id>[a-fA-F0-9]{8}-[a-fA-F0-9]{4}-4[a-fA-F0-9]{3}-[89AB][a-fA-F0-9]{3}-[a-fA-F0-9]{12})/?$", RegexOptions.NonBacktracking | RegexOptions.IgnoreCase, matchTimeoutMilliseconds: 1000)]
    private static partial Regex cookbookUrlPathRegex();

    private readonly ComponentProvider componentProvider;
    private readonly ShowCookbooks showCookbooks;
    private readonly ShowRecipes showRecipes;

    public CookbookDetailRequestHandler(
        HTMLFileWriter htmlFileWriter,
        ComponentProvider componentProvider,
        NotFoundPageRenderer notFoundPageRenderer,
        ShowRecipes showRecipes,
        ShowCookbooks showCookbooks,
        SessionService sessionService)
        : base(htmlFileWriter, notFoundPageRenderer, sessionService)
    {
        this.componentProvider = componentProvider;
        this.showCookbooks = showCookbooks;
        this.showRecipes = showRecipes;
    }

    public override bool CanHandle(HttpListenerRequest request) =>
        request.HttpMethod == HttpMethod.Get.Method
        && cookbookUrlPathRegex().IsMatch(request.Url?.AbsolutePath ?? "");

    public override Task<string> HandleHtmlFileRequest(HttpListenerRequest request)
    {
        var pageData = ExtractDataFromRequest(request);
        if (pageData.Cookbook is null)
        {
            return ReturnNotFound();
        }

        var header = componentProvider.GetComponent<Header>();
        var recipeList = componentProvider.GetComponent<RecipeList>();
        var cookbookDetailPage = componentProvider.GetComponent<CookbookDetailPage>();

        recipeList.Recipes = pageData.Recipes;
        header.CurrentChef = pageData.CurrentChef;
        cookbookDetailPage.Cookbook = pageData.Cookbook;
        cookbookDetailPage.CurrentChef = pageData.CurrentChef;
        cookbookDetailPage.SlottedChildren[RecipeDetailPage.HEADER_SLOT] = header;
        cookbookDetailPage.Children = [recipeList];
        return cookbookDetailPage.RenderAsync();
    }

    private CookbookDetailPageData ExtractDataFromRequest(HttpListenerRequest request)
    {
        var currentChef = sessionService.GetCurrentChef(request);
        var cookbookId = GetCookbookIdFromRequest(request);
        var cookbook = showCookbooks.ShowSingleCookbook(cookbookId, currentChef);
        var recipes = cookbook is null ? [] : showRecipes.ShowRecipesForCookbook(cookbook, currentChef);

        return new CookbookDetailPageData(
            currentChef,
            cookbook,
            recipes
        );
    }

    private Identifier GetCookbookIdFromRequest(HttpListenerRequest request)
    {
        return Identifier.Parse(cookbookUrlPathRegex().Match(request.Url!.AbsolutePath).Groups["cookbook_id"].Value);
    }

    private record struct CookbookDetailPageData(Chef? CurrentChef, Cookbook? Cookbook, IEnumerable<Recipe> Recipes);
}