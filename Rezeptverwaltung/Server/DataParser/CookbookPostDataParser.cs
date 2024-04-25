using Core.Data;
using Core.ValueObjects;
using Server.ContentParser;
using Server.Service;
using Server.Session;
using Server.ValueObjects.PostData;
using System.Net;

namespace Server.DataParser;

public class CookbookPostDataParser : DataParser<NewCookbookPostData>
{
    private readonly SessionService sessionService;

    public CookbookPostDataParser(
        ContentParserFactory contentParserFactory,
        HTMLSanitizer htmlSanitizer,
        SessionService sessionService)
        : base(contentParserFactory, htmlSanitizer)
    {
        this.sessionService = sessionService;
    }

    protected override Result<NewCookbookPostData> ExtractDataFromContent(IDictionary<string, ContentData> content, HttpListenerRequest request)
    {
        if (!content.TryGetValue("title", out var title) || !title!.IsText)
        {
            return GENERIC_ERROR_RESULT;
        }
        if (!content.TryGetValue("description", out var description) || !description!.IsText)
        {
            return GENERIC_ERROR_RESULT;
        }

        var visibility = Visibility.PUBLIC;
        if (content.TryGetValue("visibility", out var visibilityContent) &&
            visibilityContent!.IsText &&
            visibilityContent!.TextValue!.ToLower() == "on")
        {
            visibility = Visibility.PRIVATE;
        }

        var currentChef = sessionService.GetCurrentChef(request);
        if (currentChef is null)
        {
            return GENERIC_ERROR_RESULT;
        }

        var recipes = new List<Identifier>();
        {
            var index = 0;
            while (content.TryGetValue("recipe_" + index, out var recipeId) && recipeId!.IsText)
            {
                recipes.Add(Identifier.Parse(htmlSanitizer.Sanitize(recipeId.TextValue!)));
                index++;
            }
        }

        return Result<NewCookbookPostData>.Successful(new NewCookbookPostData(
            new Text(htmlSanitizer.Sanitize(title.TextValue!)),
            new Text(htmlSanitizer.Sanitize(description.TextValue!)),
            currentChef,
            visibility,
            recipes
        ));
    }
}