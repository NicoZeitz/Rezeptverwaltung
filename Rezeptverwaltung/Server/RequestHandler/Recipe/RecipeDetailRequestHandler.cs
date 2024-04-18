using Core.Entities;
using Core.Services;
using Core.ValueObjects;
using Core.ValueObjects.MeasurementUnits;
using Server.Components;
using Server.Session;
using System.Net;
using System.Text.RegularExpressions;

namespace Server.RequestHandler.Recipe;

public partial class RecipeDetailRequestHandler : HTMLRequestHandler
{
    private readonly RecipeDetailPage recipeDetailPage;
    private readonly Header header;
    private readonly ShowRecipes showRecipes;
    private readonly SessionService sessionService;

    [GeneratedRegex("/recipe/(?<recipe_id>[a-fA-F0-9]{8}-[a-fA-F0-9]{4}-4[a-fA-F0-9]{3}-[89AB][a-fA-F0-9]{3}-[a-fA-F0-9]{12})", RegexOptions.NonBacktracking | RegexOptions.IgnoreCase, matchTimeoutMilliseconds: 1000)]
    private static partial Regex recipeUrlPathRegex();

    public RecipeDetailRequestHandler(
        RecipeDetailPage recipeDetailPage,
        Header header,
        ShowRecipes showRecipes,
        SessionService sessionService)
    {
        this.recipeDetailPage = recipeDetailPage;
        this.header = header;
        this.showRecipes = showRecipes;
        this.sessionService = sessionService;
    }

    public bool CanHandle(HttpListenerRequest request) => recipeUrlPathRegex().IsMatch(request.Url?.AbsolutePath ?? "");

    public Task<string> HandleHtmlFileRequest(HttpListenerRequest request)
    {
        var recipeId = GetRecipeIdFromRequest(request);

        // TODO: real impl
        var recipe = new Core.Entities.Recipe(
            recipeId,
            new Username("FabianWolf"),
            new Text("Spaghetti Carbonara"),
            new Text("Spaghetti Carbonara ist ein Klassiker der italienischen Küche."),
            Visibility.PUBLIC,
            new Portion(new Rational<int>(4, 1)),
            new Duration(TimeSpan.FromMinutes(30)),
            new List<Tag>
            {
                new Tag("Spaghetti"),
                new Tag("Carbonara"),
                new Tag("Eier"),
                new Tag("Speck"),
                new Tag("Parmesan"),
                new Tag("Salz"),
                new Tag("Pfeffer")
            },
            new List<PreparationStep>
            {
                new PreparationStep(new Text("Lorem ipsum dolor sit amet, consectetur adipisicing elit. Dolores dolorem cupiditate hic saepe modi. Asperiores aperiam eveniet commodi ipsa consequuntur harum cum quaerat, inventore nemo quo qui voluptas sed repellendus.")),
                new PreparationStep(new Text("Lorem ipsum dolor sit amet, consectetur adipisicing elit. Dolores dolorem cupiditate hic saepe modi. Asperiores aperiam eveniet commodi ipsa consequuntur harum cum quaerat, inventore nemo quo qui voluptas sed repellendus.")),
                new PreparationStep(new Text("Lorem ipsum dolor sit amet, consectetur adipisicing elit. Dolores dolorem cupiditate hic saepe modi. Asperiores aperiam eveniet commodi ipsa consequuntur harum cum quaerat, inventore nemo quo qui voluptas sed repellendus.")),
                new PreparationStep(new Text("Lorem ipsum dolor sit amet, consectetur adipisicing elit. Dolores dolorem cupiditate hic saepe modi. Asperiores aperiam eveniet commodi ipsa consequuntur harum cum quaerat, inventore nemo quo qui voluptas sed repellendus.")),
                new PreparationStep(new Text("Lorem ipsum dolor sit amet, consectetur adipisicing elit. Dolores dolorem cupiditate hic saepe modi. Asperiores aperiam eveniet commodi ipsa consequuntur harum cum quaerat, inventore nemo quo qui voluptas sed repellendus.")),
                new PreparationStep(new Text("Lorem ipsum dolor sit amet, consectetur adipisicing elit. Dolores dolorem cupiditate hic saepe modi. Asperiores aperiam eveniet commodi ipsa consequuntur harum cum quaerat, inventore nemo quo qui voluptas sed repellendus.")),
                new PreparationStep(new Text("Lorem ipsum dolor sit amet, consectetur adipisicing elit. Dolores dolorem cupiditate hic saepe modi. Asperiores aperiam eveniet commodi ipsa consequuntur harum cum quaerat, inventore nemo quo qui voluptas sed repellendus.")),
            },
            new List<WeightedIngredient>
            {
                new WeightedIngredient(
                    Weight.FromGram(100),
                    new Text("Parmesan")
                ),
                new WeightedIngredient(
                    Weight.FromGram(150),
                    new Text("Speck")
                ),
                new WeightedIngredient(
                    Weight.FromGram(400),
                    new Text("Spaghetti")
                ),
                new WeightedIngredient(
                    new Piece(10),
                    new Text("Eier")
                ),
                new WeightedIngredient(
                    new Pinch(1),
                    new Text("Prise Salz")
                ),
                new WeightedIngredient(
                    Weight.FromGram(1),
                    new Text("Prise Pfeffer")
                )
            }
        );

        var currentChef = sessionService.GetCurrentChef(request);

        header.CurrentChef = currentChef;
        recipeDetailPage.Recipe = recipe;
        recipeDetailPage.SlottedChildren["Header"] = header;
        return recipeDetailPage.RenderAsync();
    }

    private Identifier GetRecipeIdFromRequest(HttpListenerRequest request)
    {
        return Identifier.Parse(recipeUrlPathRegex().Match(request.Url!.AbsolutePath).Groups["recipe_id"].Value);
    }
}
