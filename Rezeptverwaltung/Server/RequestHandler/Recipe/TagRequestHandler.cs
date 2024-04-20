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
    [GeneratedRegex("/tag/(?<tag_name>.+)", RegexOptions.NonBacktracking | RegexOptions.IgnoreCase, matchTimeoutMilliseconds: 1000)]
    private static partial Regex tagUrlPathRegex();

    private readonly TagPage tagPage;
    private readonly Header header;
    private readonly RecipeList recipeList;
    private readonly ShowRecipes showRecipes;
    private readonly SessionService sessionService;

    public TagRequestHandler(
        HTMLFileWriter htmlFileWriter,
        TagPage tagPage,
        Header header,
        RecipeList recipeList,
        ShowRecipes showRecipes,
        SessionService sessionService)
        : base(htmlFileWriter)
    {
        this.tagPage = tagPage;
        this.header = header;
        this.recipeList = recipeList;
        this.showRecipes = showRecipes;
        this.sessionService = sessionService;
    }

    public override bool CanHandle(HttpListenerRequest request) =>
        request.HttpMethod == HttpMethod.Get.Method
        && tagUrlPathRegex().IsMatch(request.Url?.AbsolutePath ?? "");

    public override Task<string> HandleHtmlFileRequest(HttpListenerRequest request)
    {
        var tag = GetTagFromRequest(request);
        var currentChef = sessionService.GetCurrentChef(request);
        var recipes = showRecipes.ShowRecipesForTag(tag, currentChef);

        header.CurrentChef = currentChef;
        recipeList.Recipes = recipes;
        tagPage.Tag = tag;
        tagPage.Children = new List<Component>() { recipeList };
        tagPage.SlottedChildren["Header"] = header;
        return tagPage.RenderAsync();
    }

    private Tag GetTagFromRequest(HttpListenerRequest request)
    {
        var match = tagUrlPathRegex().Match(request.Url?.AbsolutePath ?? "");
        var tagName = match.Groups["tag_name"].Value;
        return new Tag(tagName);
    }
}