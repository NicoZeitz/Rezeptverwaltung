﻿using Core.Entities;

namespace Server.Components;



public class RecipeList : TemplateComponent
{
    public IEnumerable<Recipe> Recipes { get; set; } = Enumerable.Empty<Recipe>();

    private readonly ImageUrlService imageUrlService;

    public RecipeList(ResourceLoader.ResourceLoader resourceLoader, ImageUrlService imageUrlService)
        : base(resourceLoader)
    {
        this.imageUrlService = imageUrlService;
    }

    public override Task<string> RenderAsync()
    {
        var recipes = Recipes.Select(recipe => new RecipeWithImage(
            recipe,
            imageUrlService.GetImageUrlForRecipe(recipe)
        ));

        return templateLoader
            .LoadTemplate("RecipeList.html")!
            .RenderAsync(new
            {
                Recipes = recipes
            })
            .AsTask();
    }

    private record struct RecipeWithImage(Recipe Recipe, string Image);
}
