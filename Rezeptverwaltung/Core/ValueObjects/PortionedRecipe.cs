using Core.Entities;

namespace Core.ValueObjects;

public record class PortionedRecipe(Recipe Recipe, Portion Portion)
{
}
