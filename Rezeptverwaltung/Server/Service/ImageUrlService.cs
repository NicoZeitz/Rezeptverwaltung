using Core.Entities;

namespace Server.Service;

public class ImageUrlService
{
    public string GetImageUrlForChef(Chef chef)
    {
        return $"/images/chef/{chef.Username.Name}";
    }

    public string GetImageUrlForRecipe(Recipe recipe)
    {
        return $"/images/recipe/{recipe.Identifier}";
    }
}
