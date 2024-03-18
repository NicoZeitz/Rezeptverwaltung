using Core.ValueObjects;

namespace Core.Entities
{
    public class Recipe
    {
        public Identifier ChefId { get; }
        public Text Title { get; }
        public Text Description { get; }
        public List<Tag> Tags { get; }
        public Portion Portion { get; }
        public Duration PreparationTime { get; }
        public List<PreparationStep> PreparationSteps { get; }
        public List<WeightedIngredient> WeightedIngredients { get; }
        public Image DishImage { get; }

        public Recipe(Identifier chefId, Duration preparationTime)
        {
            ChefId = chefId;
            PreparationTime = preparationTime;
        }

        public static string Test()
        {
            // TODO: Remove
            return "Hello Nico & Fabian!";
        }
    }
}
