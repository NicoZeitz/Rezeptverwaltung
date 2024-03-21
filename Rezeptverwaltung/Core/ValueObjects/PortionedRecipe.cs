using Core.Entities;

namespace Core.ValueObjects;

public sealed record class PortionedRecipe(Recipe Recipe, Portion Portion);
