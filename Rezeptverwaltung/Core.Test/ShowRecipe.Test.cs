using Core.Entities;
using Core.Repository;
using Core.Services;
using Core.ValueObjects;
using FluentAssertions;
using Moq;

namespace Core.Test;

public class ShowRecipeTest
{
    [Fact]
    public void GuestShouldOnlySeePublicRecipes()
    {
        Chef? loggedInChef = null;

        var publicRecipe = GetPublicRecipe();
        var privateRecipe = GetPrivateRecipe();
        var allRecipes = new List<Recipe>() {
            publicRecipe,
            privateRecipe
        };

        var recipeRepositoryMock = new Mock<RecipeRepository>();
        recipeRepositoryMock.Setup(library => library.FindAll())
            .Returns(allRecipes);

        var showRecipes = new ShowRecipes(recipeRepositoryMock.Object);

        var recipeList = showRecipes.ShowAllRecipes(loggedInChef).ToList();

        recipeList.Should().Contain(publicRecipe);
        recipeList.Should().NotContain(privateRecipe);
        recipeRepositoryMock.Verify(library => library.FindAll(), Times.Once);
    }

    [Fact]
    public void ChefShouldSeeAllOwnRecipes()
    {
        var loggedInChef = GetSampleChef(new Username("Test"));

        var ownPublicRecipe = GetPublicRecipeFrom(loggedInChef);
        var ownPrivateRecipe = GetPrivateRecipeFrom(loggedInChef);

        var allRecipes = new List<Recipe>() {
            ownPublicRecipe,
            ownPrivateRecipe,
        };

        var recipeRepositoryMock = new Mock<RecipeRepository>();
        recipeRepositoryMock.Setup(library => library.FindAll())
            .Returns(allRecipes);

        var showRecipes = new ShowRecipes(recipeRepositoryMock.Object);

        var recipeList = showRecipes.ShowAllRecipes(loggedInChef).ToList();

        recipeList.Should().Contain(ownPublicRecipe);
        recipeList.Should().Contain(ownPrivateRecipe);
        recipeRepositoryMock.Verify(library => library.FindAll(), Times.Once);
    }

    [Fact]
    public void ChefShouldSeeOnlyOtherPublicRecipes()
    {
        var loggedInChef = GetSampleChef(new Username("Test"));
        var otherChef = GetSampleChef(new Username("Other"));

        var otherPublicRecipe = GetPublicRecipeFrom(otherChef);
        var otherPrivateRecipe = GetPrivateRecipeFrom(otherChef);

        var allRecipes = new List<Recipe>() {
            otherPublicRecipe,
            otherPrivateRecipe,
        };

        var recipeRepositoryMock = new Mock<RecipeRepository>();
        recipeRepositoryMock.Setup(library => library.FindAll())
            .Returns(allRecipes);

        var showRecipes = new ShowRecipes(recipeRepositoryMock.Object);

        var recipeList = showRecipes.ShowAllRecipes(loggedInChef).ToList();

        recipeList.Should().Contain(otherPublicRecipe);
        recipeList.Should().NotContain(otherPrivateRecipe);
        recipeRepositoryMock.Verify(library => library.FindAll(), Times.Once);
    }

    private Recipe GetPublicRecipe() => GetPublicRecipeFrom(null);
    private Recipe GetPrivateRecipe() => GetPrivateRecipeFrom(null);

    private Recipe GetPublicRecipeFrom(Chef? chef = null)
    {
        var recipe = GetSampleRecipe(Identifier.Parse("35d2ec2f-1a56-4783-ad4b-c771fe128760"), chef);
        recipe.Visibility = Visibility.PUBLIC;
        return recipe;
    }

    private Recipe GetPrivateRecipeFrom(Chef? chef = null)
    {
        var recipe = GetSampleRecipe(Identifier.Parse("1a52f22f-ceba-456f-84b5-19f9e6c0d8b9"), chef);
        recipe.Visibility = Visibility.PRIVATE;
        return recipe;
    }

    private Recipe GetSampleRecipe(Identifier id, Chef? chef = null)
    {
        var username = chef?.Username ?? new Username("");
        return new Recipe(
            id,
            username,
            new Text(""),
            new Text(""),
            Visibility.PUBLIC,
            new Portion(new Rational<int>(1, 1)),
            new Duration(TimeSpan.Zero),
            [],
            [],
            []
        );
    }

    private Chef GetSampleChef(Username username)
    {
        return new Chef(username, new Name("", ""), new HashedPassword(""));
    }
}