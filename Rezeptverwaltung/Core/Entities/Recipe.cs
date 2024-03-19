using Core.ValueObjects;

namespace Core.Entities
{
    public class Recipe
    {
        public Identifier Identifier { get; }
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

        public virtual bool Equals(Recipe? other)
        {
            if (ReferenceEquals(this, other))
                return true;

            if (other is null)
                return false;

            return Identifier == other.Identifier;
        }

        public override int GetHashCode() => Identifier.GetHashCode();

        public override bool Equals(object? obj) => Equals(obj as Recipe);

        public static bool operator ==(Recipe? left, Recipe? right) =>
            ReferenceEquals(left, right) || (left is not null && left.Equals(right));

        public static bool operator !=(Recipe? left, Recipe? right) => !(left == right);
    }
}
