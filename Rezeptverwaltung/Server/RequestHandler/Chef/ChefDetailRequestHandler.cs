using Core.Entities;
using Core.ValueObjects;
using Core.ValueObjects.MeasurementUnits;
using Server.Components;
using Server.Session;
using System.Net;
using System.Text.RegularExpressions;

namespace Server.RequestHandler.Chef;

public partial class ChefDetailRequestHandler : HTMLRequestHandler
{
    private readonly ChefDetailPage chefDetailPage;
    private readonly Header header;
    private readonly RecipeList recipeList;
    private readonly SessionService sessionService;

    [GeneratedRegex("/chef/(?<chef_username>[A-Z0-9_ ]+)", RegexOptions.NonBacktracking | RegexOptions.IgnoreCase, matchTimeoutMilliseconds: 1000)]
    private static partial Regex chefDetailUrlPathRegex();

    public ChefDetailRequestHandler(
        ChefDetailPage chefDetailPage,
        Header header,
        RecipeList recipeList,
        SessionService sessionService)
    {
        this.chefDetailPage = chefDetailPage;
        this.header = header;
        this.recipeList = recipeList;
        this.sessionService = sessionService;
    }

    public bool CanHandle(HttpListenerRequest request) => chefDetailUrlPathRegex().IsMatch(request.Url?.AbsolutePath ?? "");

    public Task<string> HandleHtmlFileRequest(HttpListenerRequest request)
    {
        var username = GetChefUsernameFromRequest(request);

        // TODO: real implementation
        var chef = new Core.Entities.Chef(
            username,
            new Core.ValueObjects.Name("Fabian", "Wolf"),
            new Core.ValueObjects.HashedPassword("1234")
        );
        var recipes = new List<Core.Entities.Recipe>() {
            new Core.Entities.Recipe(
            Identifier.NewId(),
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
        ),
            new Core.Entities.Recipe(
            Identifier.NewId(),
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
            )
        };

        var currentChef = sessionService.GetCurrentChef(request);

        header.CurrentChef = currentChef;
        recipeList.Recipes = recipes;
        chefDetailPage.Chef = chef;
        chefDetailPage.SlottedChildren["Recipes"] = recipeList;
        chefDetailPage.SlottedChildren["Header"] = header;
        return chefDetailPage.RenderAsync();
    }

    private Username GetChefUsernameFromRequest(HttpListenerRequest request)
    {
        return new Username(chefDetailUrlPathRegex().Match(request.Url!.AbsolutePath).Groups["chef_username"].Value);
    }
}
