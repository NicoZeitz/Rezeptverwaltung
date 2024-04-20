using Core.Data;
using Core.Services;
using Core.ValueObjects;
using Server.ContentParser;

namespace Server.RequestHandler;

public class RecipePostDataParser : DataParser<RecipePostData>
{
    private readonly MeasurementUnitSerializationManager measurementUnitSerializationManager;

    public RecipePostDataParser(ContentParserFactory contentParserFactory, MeasurementUnitSerializationManager measurementUnitSerializationManager)
        : base(contentParserFactory)
    {
        this.measurementUnitSerializationManager = measurementUnitSerializationManager;
    }

    public override Result<RecipePostData> ExtractDataFromContent(IDictionary<string, ContentData> content)
    {
        if (!content.TryGetValue("title", out var title) && title!.IsText)
        {
            return GENERIC_ERROR_RESULT;
        }
        if (!content.TryGetValue("description", out var description) && description!.IsText)
        {
            return GENERIC_ERROR_RESULT;
        }
        var tags = new List<Tag>();
        {
            var index = 0;
            while (content.TryGetValue("tag_" + index, out var tag) && tag!.IsText)
            {
                tags.Add(new Tag(tag.TextValue!));
                index++;
            }
        }

        var ingredients = new List<Ingredient>();
        {
            var index = 0;
            while (
                content.TryGetValue("ingredient_" + index + "_amount", out var amount) && amount!.IsText &&
                content.TryGetValue("ingredient_" + index + "_unit", out var unit) && unit!.IsText &&
                content.TryGetValue("ingredient_" + index + "_name", out var name) && name!.IsText)
            {
                var measurementUnit = measurementUnitSerializationManager.DeserializeFrom(new SerializedMeasurementUnit(
                    name.TextValue!,
                    unit.TextValue!,
                    amount.TextValue!
                ));

                // ingredients.Add(
                //     new WeightedIngredient(
                //        measurementUnit,
                //           name.TextValue!
                //     )
                // );
                index++;
            }
        }




        // TODO:
        if (!content.TryGetValue("steps", out var steps) && steps!.IsText)
        {
            return GENERIC_ERROR_RESULT;
        }
        if (!content.TryGetValue("image", out var image) && image!.IsFile)
        {
            return GENERIC_ERROR_RESULT;
        }

        // return Result<RecipePostData>.Successful(new RecipePostData(
        //     title.TextValue!,
        //     description.TextValue!,
        //     tags,
        //     ingredients.TextValue!,
        //     steps.TextValue!,
        //     image!
        // ));
        return GENERIC_ERROR_RESULT;
    }
}