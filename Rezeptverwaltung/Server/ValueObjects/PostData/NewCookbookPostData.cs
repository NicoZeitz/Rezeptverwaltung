using Core.Entities;
using Core.ValueObjects;

namespace Server.ValueObjects.PostData;

public record struct NewCookbookPostData(
    Text Title,
    Text Description,
    Chef chef,
    Visibility Visibility,
    IEnumerable<Identifier> Recipes
);