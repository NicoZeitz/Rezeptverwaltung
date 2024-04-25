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

public partial class RecipeDetailRequestHandler : HTMLRequestHandler
{
    private readonly ComponentProvider componentProvider;
    private readonly ShowRecipes showRecipes;

    [GeneratedRegex("^/recipe/(?<recipe_id>[a-fA-F0-9]{8}-[a-fA-F0-9]{4}-4[a-fA-F0-9]{3}-[89AB][a-fA-F0-9]{3}-[a-fA-F0-9]{12})/?$", RegexOptions.NonBacktracking | RegexOptions.IgnoreCase, matchTimeoutMilliseconds: 1000)]
    private static partial Regex recipeUrlPathRegex();

    public RecipeDetailRequestHandler(
        ComponentProvider componentProvider,
        HTMLFileWriter htmlFileWriter,
        NotFoundPageRenderer notFoundPageRenderer,
        SessionService sessionService,
        ShowRecipes showRecipes)
        : base(htmlFileWriter, notFoundPageRenderer, sessionService)
    {
        this.componentProvider = componentProvider;
        this.showRecipes = showRecipes;
    }

    public override bool CanHandle(HttpListenerRequest request) =>
        request.HttpMethod == HttpMethod.Get.Method
        && recipeUrlPathRegex().IsMatch(request.Url?.AbsolutePath ?? "");

    public override Task<string> HandleHtmlFileRequest(HttpListenerRequest request)
    {
        var pageData = ExtractDataFromRequest(request);
        if (pageData.Recipe is null)
        {
            return ReturnNotFound();
        }

        var header = componentProvider.GetComponent<Header>();
        var recipeDetailPage = componentProvider.GetComponent<RecipeDetailPage>();

        header.CurrentChef = pageData.CurrentChef;
        recipeDetailPage.Recipe = pageData.Recipe;
        recipeDetailPage.CurrentChef = pageData.CurrentChef;
        recipeDetailPage.SlottedChildren[RecipeDetailPage.HEADER_SLOT] = header;
        return recipeDetailPage.RenderAsync();
    }

    private RecipeDetailPageData ExtractDataFromRequest(HttpListenerRequest request)
    {
        var currentChef = sessionService.GetCurrentChef(request);
        var recipeId = GetRecipeIdFromRequest(request);
        var recipe = showRecipes.ShowSingleRecipe(recipeId, currentChef);

        return new RecipeDetailPageData(
            currentChef,
            recipe
        );
    }

    private Identifier GetRecipeIdFromRequest(HttpListenerRequest request)
    {
        return Identifier.Parse(recipeUrlPathRegex().Match(request.Url!.AbsolutePath).Groups["recipe_id"].Value);
    }

    private record struct RecipeDetailPageData(Chef? CurrentChef, Recipe? Recipe);
}
