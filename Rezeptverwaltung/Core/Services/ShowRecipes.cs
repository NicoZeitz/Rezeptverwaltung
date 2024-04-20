﻿using Core.Entities;
using Core.Repository;
using Core.Services.Retrieval;
using Core.ValueObjects;

namespace Core.Services;

public class ShowRecipes
{
    private readonly RecipeRepository recipeRepository;

    public ShowRecipes(RecipeRepository recipeRepository) : base()
    {
        this.recipeRepository = recipeRepository;
    }

    public Recipe? ShowSingleRecipe(Identifier identifier, Chef? viewer)
    {
        var retrievalGraph = CreateRetrievalGraph(
            viewer,
            new OptionalSingleItemListRetrieval<Recipe>(
                recipeRepository.FindByIdentifier(identifier)
            )
        );

        return retrievalGraph
            .Retrieve()
            .FirstOrDefault();
    }

    public IEnumerable<Recipe> ShowRecipesForTag(Tag tag, Chef? viewer)
    {
        var retrievalGraph = CreateRetrievalGraph(
            viewer,
            new SimpleListRetrieval<Recipe>(recipeRepository.FindByTag(tag))
        );
        return retrievalGraph.Retrieve();
    }

    public IEnumerable<Recipe> ShowAllRecipes(Chef? viewer)
    {
        var retrievalGraph = CreateRetrievalGraph(
            viewer,
            new SimpleListRetrieval<Recipe>(
                recipeRepository.FindAll()
            )
        );
        return retrievalGraph.Retrieve();
    }

    public IEnumerable<Recipe> ShowRecipesForChef(Chef chef, Chef? viewer)
    {
        var retrievalGraph = CreateRetrievalGraph(
            viewer,
            new SimpleListRetrieval<Recipe>(
                recipeRepository.FindForChef(chef)
            )
        );
        return retrievalGraph.Retrieve();
    }

    public IEnumerable<Recipe> ShowRecipesForShoppingList(ShoppingList shoppingList, Chef? viewer)
    {
        var retrievalGraph = CreateRetrievalGraph(
            viewer,
            new SimpleListRetrieval<Recipe>(
                recipeRepository.FindForShoppingList(shoppingList)
            )
        );
        return retrievalGraph.Retrieve();
    }

    public IEnumerable<Recipe> ShowRecipesForCookbook(Cookbook cookbook, Chef? viewer)
    {
        var retrievalGraph = CreateRetrievalGraph(
            viewer,
            new SimpleListRetrieval<Recipe>(
                recipeRepository.FindForCookbook(cookbook)
            )
        );
        return retrievalGraph.Retrieve();
    }

    private ListRetrieval<Recipe> CreateRetrievalGraph(Chef? viewer, ListRetrieval<Recipe> baseRetriever)
    {
        var filterAccessRights = new FilterAccessRights<Recipe>(viewer, baseRetriever);
        var orderByProperty = new OrderByProperty<Recipe>(recipe => recipe.Title, filterAccessRights);
        return orderByProperty;
    }
}
