using Core.Services;
using Core.ValueObjects;
using Server.Components;
using Server.PageRenderer;
using Server.Service;
using Server.Session;
using System.Net;
using System.Text.RegularExpressions;

namespace Server.RequestHandler;

public partial class SearchRequestHandler : HTMLRequestHandler
{
    [GeneratedRegex("^?search=(?<search_query>.+)$", RegexOptions.NonBacktracking | RegexOptions.IgnoreCase, matchTimeoutMilliseconds: 1000)]
    private static partial Regex tagUrlPathRegex();

    private readonly ComponentProvider componentProvider;
    private readonly ShowRecipes showRecipes;
    private readonly URLEncoder urlEncoder;

    public SearchRequestHandler(
        HTMLFileWriter htmlFileWriter,
        ComponentProvider componentProvider,
        ShowRecipes showRecipes,
        SessionService sessionService,
        NotFoundPageRenderer notFoundPageRenderer,
        URLEncoder urlEncoder)
        : base(htmlFileWriter, notFoundPageRenderer, sessionService)
    {
        this.componentProvider = componentProvider;
        this.showRecipes = showRecipes;
        this.urlEncoder = urlEncoder;
    }

    public override bool CanHandle(HttpListenerRequest request) =>
        request.HttpMethod == HttpMethod.Get.Method &&
        request.Url?.AbsolutePath == "/" &&
        tagUrlPathRegex().IsMatch(request.Url?.Query ?? "");

    public override Task<string> HandleHtmlFileRequest(HttpListenerRequest request)
    {
        var searchTerm = GetSearchQueryFromRequest(request);
        var currentChef = sessionService.GetCurrentChef(request);

        var recipes = showRecipes.ShowRecipesForQuery(searchTerm, currentChef);

        var header = componentProvider.GetComponent<Header>();
        var recipeList = componentProvider.GetComponent<RecipeList>();
        var searchPage = componentProvider.GetComponent<SearchPage>();

        header.CurrentChef = currentChef;
        recipeList.Recipes = recipes;
        searchPage.SearchTerm = searchTerm;
        searchPage.Children = [recipeList];
        searchPage.SlottedChildren[TagPage.HEADER_SLOT] = header;
        return searchPage.RenderAsync();
    }

    private Text GetSearchQueryFromRequest(HttpListenerRequest request)
    {
        var match = tagUrlPathRegex().Match(request.Url?.Query ?? "");
        var tagName = match.Groups["search_query"].Value;
        tagName = urlEncoder.URLDecode(tagName);
        return new Text(tagName);
    }
}