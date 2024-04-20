using Core.Services;
using Core.ValueObjects;
using Server.Components;
using Server.Service;
using Server.Session;
using System.Net;
using System.Text.RegularExpressions;

namespace Server.RequestHandler;

public partial class TagRequestHandler : HTMLRequestHandler
{
    [GeneratedRegex("^/tag/(?<tag_name>.+)/?$", RegexOptions.NonBacktracking | RegexOptions.IgnoreCase, matchTimeoutMilliseconds: 1000)]
    private static partial Regex tagUrlPathRegex();

    private readonly ComponentProvider componentProvider;
    private readonly ShowRecipes showRecipes;
    private readonly SessionService sessionService;
    private readonly URLEncoder urlEncoder;

    public TagRequestHandler(
        HTMLFileWriter htmlFileWriter,
        ComponentProvider componentProvider,
        ShowRecipes showRecipes,
        SessionService sessionService,
        URLEncoder urlEncoder)
        : base(htmlFileWriter)
    {
        this.componentProvider = componentProvider;
        this.showRecipes = showRecipes;
        this.sessionService = sessionService;
        this.urlEncoder = urlEncoder;
    }

    public override bool CanHandle(HttpListenerRequest request) =>
        request.HttpMethod == HttpMethod.Get.Method
        && tagUrlPathRegex().IsMatch(request.Url?.AbsolutePath ?? "");

    public override Task<string> HandleHtmlFileRequest(HttpListenerRequest request)
    {
        var tag = GetTagFromRequest(request);
        var currentChef = sessionService.GetCurrentChef(request);
        var recipes = showRecipes.ShowRecipesForTag(tag, currentChef);

        var header = componentProvider.GetComponent<Header>();
        var recipeList = componentProvider.GetComponent<RecipeList>();
        var tagPage = componentProvider.GetComponent<TagPage>();

        header.CurrentChef = currentChef;
        recipeList.Recipes = recipes;
        tagPage.Tag = tag;
        tagPage.Children = [recipeList];
        tagPage.SlottedChildren["Header"] = header;
        return tagPage.RenderAsync();
    }

    private Tag GetTagFromRequest(HttpListenerRequest request)
    {
        var match = tagUrlPathRegex().Match(request.Url?.AbsolutePath ?? "");
        var tagName = match.Groups["tag_name"].Value;
        tagName = urlEncoder.URLDecode(tagName);
        return new Tag(tagName);
    }
}