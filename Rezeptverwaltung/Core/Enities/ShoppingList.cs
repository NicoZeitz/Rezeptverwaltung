using Core.ValueObjects;

namespace Core.Enities
{
    public class ShoppingList
    {

        public List<Recipe> Recipes { get; set; }
        public List<Ingredient> Ingredients { get; set;}

    }
}
