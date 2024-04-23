using Core.ValueObjects;

namespace Server.ValueObjects.PostData;

public record struct NewCookbookPostData(
    Text Title,
    Text Description,
    Visibility Visibility,
    IEnumerable<Identifier> Recipes
);