using core.Services;
using Core.Data;
using Core.Services.Serialization;
using Core.ValueObjects;
using Core.ValueObjects.MeasurementUnits;
using Server.ContentParser;
using Server.Service;
using Server.Session;
using Server.ValueObjects.PostData;
using System.Net;

namespace Server.DataParser;

public class RecipePostDataParser : DataParser<NewRecipePostData>
{
    private readonly MeasurementUnitSerializationManager measurementUnitSerializationManager;
    private readonly ReduceFractionService<int> reduceFractionService;
    private readonly SessionService sessionService;

    public RecipePostDataParser(
        ContentParserFactory contentParserFactory,
        HTMLSanitizer htmlSanitizer,
        MeasurementUnitSerializationManager measurementUnitSerializationManager,
        ReduceFractionService<int> reduceFractionService,
        SessionService sessionService)
        : base(contentParserFactory, htmlSanitizer)
    {
        this.measurementUnitSerializationManager = measurementUnitSerializationManager;
        this.reduceFractionService = reduceFractionService;
        this.sessionService = sessionService;
    }

    protected override Result<NewRecipePostData> ExtractDataFromContent(IDictionary<string, ContentData> content, HttpListenerRequest request)
    {
        if (!content.TryGetValue("title", out var title) || !title!.IsText)
        {
            return GENERIC_ERROR_RESULT;
        }
        if (!content.TryGetValue("description", out var description) || !description!.IsText)
        {
            return GENERIC_ERROR_RESULT;
        }

        if (!content.TryGetValue("portion_denominator", out var portionDenominator) || !portionDenominator!.IsText)
        {
            return GENERIC_ERROR_RESULT;
        }
        if (!content.TryGetValue("portion_numerator", out var portionNumerator) || !portionNumerator!.IsText)
        {
            return GENERIC_ERROR_RESULT;
        }
        if (!int.TryParse(portionDenominator.TextValue!, out var portionDenominatorValue))
        {
            return GENERIC_ERROR_RESULT;
        }
        if (!int.TryParse(portionNumerator.TextValue!, out var portionNumeratorValue))
        {
            return GENERIC_ERROR_RESULT;
        }

        var portion = new Portion(reduceFractionService.ReduceFraction(new Rational<int>(
            portionNumeratorValue,
            portionDenominatorValue
        )));

        if (!content.TryGetValue("duration", out var durationContent) || !durationContent!.IsText)
        {
            return GENERIC_ERROR_RESULT;
        }

        var duration = Duration.Parse(htmlSanitizer.Sanitize(durationContent.TextValue!));
        if (duration is null)
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

        var tags = new List<Tag>();
        {
            var index = 0;
            while (content.TryGetValue("tags_" + index, out var tag) && tag!.IsText)
            {
                tags.Add(new Tag(htmlSanitizer.Sanitize(tag.TextValue!)));
                index++;
            }
        }

        var ingredients = new List<WeightedIngredient>();
        {
            var index = 0;
            while (
                content.TryGetValue("ingredient_" + index + "_amount", out var amount) && amount!.IsText &&
                content.TryGetValue("ingredient_" + index + "_unit", out var unit) && unit!.IsText &&
                content.TryGetValue("ingredient_" + index + "_name", out var name) && name!.IsText)
            {
                var measurementUnit = measurementUnitSerializationManager.DeserializeFrom(new SerializedMeasurementUnit(
                    htmlSanitizer.Sanitize(unit.TextValue!),
                    htmlSanitizer.Sanitize(amount.TextValue!)
                ));
                if (measurementUnit is null)
                {
                    return GENERIC_ERROR_RESULT;
                }

                var ingredient = new Ingredient(htmlSanitizer.Sanitize(name.TextValue!));

                var weightedIngredient = new WeightedIngredient(
                    measurementUnit,
                    ingredient
                );

                ingredients.Add(weightedIngredient);
                index++;
            }
        }

        var preparationSteps = new List<PreparationStep>();
        {
            var index = 0;
            while (content.TryGetValue("preparation_step_" + index, out var preparationStepText) && preparationStepText!.IsText)
            {
                var preparationStep = new PreparationStep(new Text(htmlSanitizer.Sanitize(preparationStepText.TextValue!)));
                preparationSteps.Add(preparationStep);
                index++;
            }
        }
        if (preparationSteps.Count == 0)
        {
            return Result<NewRecipePostData>.Error(ErrorMessages.NO_PREPARATION_STEPS);
        }

        if (!content.TryGetValue("profile_image", out var image) || !image!.IsFile)
        {
            return GENERIC_ERROR_RESULT;
        }

        return Result<NewRecipePostData>.Successful(new NewRecipePostData(
            new Text(htmlSanitizer.Sanitize(title.TextValue!)),
            new Text(htmlSanitizer.Sanitize(description.TextValue!)),
            portion,
            duration.Value,
            visibility,
            currentChef.Username,
            tags,
            ingredients,
            preparationSteps,
            image!
        ));
    }
}