using Core.Data;
using Core.ValueObjects;
using Server.ContentParser;
using Server.Service;
using Server.Session;
using Server.ValueObjects.PostData;
using System.Net;

namespace Server.DataParser;

public class ShoppingListPostDataParser : DataParser<NewShoppingListPostData>
{
    private readonly SessionService sessionService;

    public ShoppingListPostDataParser(
        ContentParserFactory contentParserFactory,
        HTMLSanitizer htmlSanitizer,
        SessionService sessionService)
        : base(contentParserFactory, htmlSanitizer)
    {
        this.sessionService = sessionService;
    }

    protected override Result<NewShoppingListPostData> ExtractDataFromContent(IDictionary<string, ContentData> content, HttpListenerRequest request)
    {
        if (!content.TryGetValue("title", out var title) || !title!.IsText)
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

        var recipes = new List<PortionedRecipe>();
        {
            var index = 0;
            while (
                content.TryGetValue("recipe_" + index, out var recipeId) && recipeId!.IsText &&
                content.TryGetValue("portion_denominator_" + index, out var portionDenominator) && portionDenominator!.IsText &&
                content.TryGetValue("portion_numerator_" + index, out var portionNumerator) && portionNumerator!.IsText)
            {
                var recipeIdentifier = Identifier.Parse(htmlSanitizer.Sanitize(recipeId.TextValue!));
                if (recipeIdentifier is null)
                {
                    break;
                }

                if (!int.TryParse(portionDenominator.TextValue!, out var portionDenominatorValue))
                {
                    return GENERIC_ERROR_RESULT;
                }
                if (!int.TryParse(portionNumerator.TextValue!, out var portionNumeratorValue))
                {
                    return GENERIC_ERROR_RESULT;
                }

                var portion = new Portion(new Rational<int>(
                    portionDenominatorValue,
                    portionNumeratorValue
                ));
                recipes.Add(new PortionedRecipe(recipeIdentifier.Value, portion));
                index++;
            }
        }

        return Result<NewShoppingListPostData>.Successful(new NewShoppingListPostData(
            new Text(htmlSanitizer.Sanitize(title.TextValue!)),
            currentChef,
            visibility,
            recipes
        ));
    }
}