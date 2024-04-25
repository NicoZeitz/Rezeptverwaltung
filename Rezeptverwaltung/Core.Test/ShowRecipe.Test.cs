using Core.Entities;
using Core.Repository;
using Core.Services;
using Core.ValueObjects;
using FluentAssertions;
using Moq;

namespace Core.Test;

public class ShowRecipeTest
{
    public class ShowAllRecipes
    {
        [Fact]
        public void GuestSeeOnlyPublicRecipes()
        {
            Chef? loggedInChef = null;

            var publicRecipe = GetPublicRecipe();
            var privateRecipe = GetPrivateRecipe();
            var allRecipes = new List<Recipe>() {
                publicRecipe,
                privateRecipe
            };

            var recipeRepositoryMock = new Mock<RecipeRepository>();
            recipeRepositoryMock
                .Setup(recipeRepository => recipeRepository.FindAll())
                .Returns(allRecipes);

            var showRecipes = new ShowRecipes(recipeRepositoryMock.Object);

            var recipeList = showRecipes.ShowAllRecipes(loggedInChef).ToList();

            recipeList.Should().Contain(publicRecipe);
            recipeList.Should().NotContain(privateRecipe);
            recipeRepositoryMock.Verify(recipeRepository => recipeRepository.FindAll(), Times.Once);
        }

        [Fact]
        public void ChefSeesAllOwnRecipes()
        {
            var loggedInChef = GetSampleChef(new Username("Test"));

            var ownPublicRecipe = GetPublicRecipeFrom(loggedInChef);
            var ownPrivateRecipe = GetPrivateRecipeFrom(loggedInChef);

            var allRecipes = new List<Recipe>() {
                ownPublicRecipe,
                ownPrivateRecipe,
            };

            var recipeRepositoryMock = new Mock<RecipeRepository>();
            recipeRepositoryMock
                .Setup(recipeRepository => recipeRepository.FindAll())
                .Returns(allRecipes);

            var showRecipes = new ShowRecipes(recipeRepositoryMock.Object);

            var recipeList = showRecipes.ShowAllRecipes(loggedInChef).ToList();

            recipeList.Should().Contain(ownPublicRecipe);
            recipeList.Should().Contain(ownPrivateRecipe);
            recipeRepositoryMock.Verify(recipeRepository => recipeRepository.FindAll(), Times.Once);
        }

        [Fact]
        public void ChefSeesOnlyOtherPublicRecipes()
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
            recipeRepositoryMock
                .Setup(recipeRepository => recipeRepository.FindAll())
                .Returns(allRecipes);

            var showRecipes = new ShowRecipes(recipeRepositoryMock.Object);

            var recipeList = showRecipes.ShowAllRecipes(loggedInChef).ToList();

            recipeList.Should().Contain(otherPublicRecipe);
            recipeList.Should().NotContain(otherPrivateRecipe);
            recipeRepositoryMock.Verify(recipeRepository => recipeRepository.FindAll(), Times.Once);
        }
    }

    public class ShowSingleRecipe
    {
        [Fact]
        public void GuestSeesPublicRecipe()
        {
            Chef? loggedInChef = null;

            var publicRecipe = GetPublicRecipe();

            var recipeRepositoryMock = new Mock<RecipeRepository>();
            recipeRepositoryMock
                .Setup(recipeRepository => recipeRepository.FindByIdentifier(publicRecipe.Identifier))
                .Returns(publicRecipe);

            var showRecipes = new ShowRecipes(recipeRepositoryMock.Object);

            var recipe = showRecipes.ShowSingleRecipe(publicRecipe.Identifier, loggedInChef);

            recipe.Should().Be(publicRecipe);
            recipeRepositoryMock.Verify(recipeRepository => recipeRepository.FindByIdentifier(publicRecipe.Identifier), Times.Once);
        }

        [Fact]
        public void GuestDoesNotSeePrivateRecipe()
        {
            Chef? loggedInChef = null;

            var privateRecipe = GetPrivateRecipe();

            var recipeRepositoryMock = new Mock<RecipeRepository>();
            recipeRepositoryMock
                .Setup(recipeRepository => recipeRepository.FindByIdentifier(privateRecipe.Identifier))
                .Returns(privateRecipe);

            var showRecipes = new ShowRecipes(recipeRepositoryMock.Object);

            var recipe = showRecipes.ShowSingleRecipe(privateRecipe.Identifier, loggedInChef);

            recipe.Should().BeNull();
            recipeRepositoryMock.Verify(recipeRepository => recipeRepository.FindByIdentifier(privateRecipe.Identifier), Times.Once);
        }

        [Fact]
        public void ChefSeesOwnPublicRecipe()
        {
            var loggedInChef = GetSampleChef(new Username("Test"));

            var ownPublicRecipe = GetPublicRecipeFrom(loggedInChef);

            var recipeRepositoryMock = new Mock<RecipeRepository>();
            recipeRepositoryMock
                .Setup(recipeRepository => recipeRepository.FindByIdentifier(ownPublicRecipe.Identifier))
                .Returns(ownPublicRecipe);

            var showRecipes = new ShowRecipes(recipeRepositoryMock.Object);

            var recipe = showRecipes.ShowSingleRecipe(ownPublicRecipe.Identifier, loggedInChef);

            recipe.Should().Be(ownPublicRecipe);
            recipeRepositoryMock.Verify(recipeRepository => recipeRepository.FindByIdentifier(ownPublicRecipe.Identifier), Times.Once);
        }

        [Fact]
        public void ChefSeesOwnPrivateRecipe()
        {
            var loggedInChef = GetSampleChef(new Username("Test"));

            var ownPrivateRecipe = GetPrivateRecipeFrom(loggedInChef);

            var recipeRepositoryMock = new Mock<RecipeRepository>();
            recipeRepositoryMock
                .Setup(recipeRepository => recipeRepository.FindByIdentifier(ownPrivateRecipe.Identifier))
                .Returns(ownPrivateRecipe);

            var showRecipes = new ShowRecipes(recipeRepositoryMock.Object);

            var recipe = showRecipes.ShowSingleRecipe(ownPrivateRecipe.Identifier, loggedInChef);

            recipe.Should().Be(ownPrivateRecipe);
            recipeRepositoryMock.Verify(recipeRepository => recipeRepository.FindByIdentifier(ownPrivateRecipe.Identifier), Times.Once);
        }
    }

    private static Recipe GetPublicRecipe() => GetPublicRecipeFrom(null);
    private static Recipe GetPrivateRecipe() => GetPrivateRecipeFrom(null);

    private static Recipe GetPublicRecipeFrom(Chef? chef = null)
    {
        var recipe = GetSampleRecipe(Identifier.Parse("35d2ec2f-1a56-4783-ad4b-c771fe128760")!.Value, chef);
        recipe.Visibility = Visibility.PUBLIC;
        return recipe;
    }

    private static Recipe GetPrivateRecipeFrom(Chef? chef = null)
    {
        var recipe = GetSampleRecipe(Identifier.Parse("1a52f22f-ceba-456f-84b5-19f9e6c0d8b9")!.Value, chef);
        recipe.Visibility = Visibility.PRIVATE;
        return recipe;
    }

    private static Recipe GetSampleRecipe(Identifier id, Chef? chef = null)
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

    private static Chef GetSampleChef(Username username)
    {
        return new Chef(username, new Name("", ""), new HashedPassword(""));
    }
}
