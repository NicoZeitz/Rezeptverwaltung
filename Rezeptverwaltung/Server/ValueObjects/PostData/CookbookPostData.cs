using Core.ValueObjects;

namespace Server.ValueObjects.PostData;

public record struct NewCookbookPostData(
    Text Title,
    Text Description,
    Visibility Visibility,
    IEnumerable<Identifier> Recipes
);

public record struct EditCookbookPostData(
    Identifier Identifier,
    Text Title,
    Text Description,
    Visibility Visibility,
    IEnumerable<Identifier> Recipes
);
