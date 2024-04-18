using Core.Entities;

namespace Server;

public class ImageUrlService
{
    public string GetImageUrlForChef(Chef chef)
    {
        return $"/images/chefs/{chef.Username.Name}";
    }

    public string GetImageUrlForRecipe(Recipe recipe)
    {
        return $"/images/recipes/{recipe.Identifier}";
    }
}
