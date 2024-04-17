using Core.Entities;

namespace Core.ValueObjects;

public sealed record class PortionedRecipe(Identifier RecipeIdentifier, Portion Portion);