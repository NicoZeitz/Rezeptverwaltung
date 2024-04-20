using Core.Interfaces;
using Core.ValueObjects;

namespace Core.Entities
{
    public class Recipe : IEquatable<Recipe>, AccessRights, UniqueIdentity
    {
        public Identifier Identifier { get; }
        public Username Chef { get; }
        public Text Title { get; }
        public Text Description { get; }
        public Visibility Visibility { get; }
        public Portion Portion { get; }
        public Duration PreparationTime { get; }
        public ISet<Tag> Tags { get; }
        public IList<PreparationStep> PreparationSteps { get; }
        public ISet<WeightedIngredient> WeightedIngredients { get; }

        public Recipe(
            Identifier id,
            Username chef,
            Text title,
            Text description,
            Visibility visibility,
            Portion portion,
            Duration preparationTime,
            IEnumerable<Tag> tags,
            IList<PreparationStep> preparationSteps,
            IEnumerable<WeightedIngredient> weightedIngredients)
        {
            Identifier = id;
            Chef = chef;
            Title = title;
            Description = description;
            Visibility = visibility;
            Portion = portion;
            PreparationTime = preparationTime;
            Tags = tags.ToHashSet();
            PreparationSteps = preparationSteps.ToList();
            WeightedIngredients = weightedIngredients.ToHashSet();
        }

        public string GetUniqueIdentity() => Identifier.Id.ToString();

        public bool IsVisibleTo(Chef viewer) => viewer.Username == Chef || Visibility.IsPublic();

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
