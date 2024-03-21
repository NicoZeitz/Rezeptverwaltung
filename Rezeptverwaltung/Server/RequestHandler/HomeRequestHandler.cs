using Core.Entities;
using Core.ValueObjects;
using Core.ValueObjects.MeasurementUnits;
using Scriban;
using Server.Component;
using Server.ResourceLoader;
using Server.Session;
using System.Net;

namespace Server.RequestHandler;

public class HomeRequestHandler : IHTMLRequestHandler
{
    private readonly IResourceLoader resourceLoader;
    private readonly ISessionService<Chef> sessionService;

    public HomeRequestHandler(IResourceLoader resourceLoader, ISessionService<Chef> sessionService)
    {
        this.resourceLoader = resourceLoader;
        this.sessionService = sessionService;
    }

    public bool CanHandle(HttpListenerRequest request)
    {
        if (request.HttpMethod != "GET")
        {
            return false;
        }

        return request.Url!.AbsolutePath is "/" or "/index.html";
    }

    public async Task<string> HandleHtmlFileRequest(HttpListenerRequest request)
    {
        var recipes = new List<Recipe>
        {
            new Recipe(
                new Identifier(Guid.NewGuid()),
                new Username("admin"),
                new Text("This is a recipe"),
                new Text("This is a really really long description. Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."),
                Visibility.PUBLIC,
                new Portion(new Rational<int>(1,1)),
                new Duration(TimeSpan.FromMinutes(20)),
                new List<Tag> {
                    new Tag("Tag 1"),
                    new Tag("Tag 2")
                },
                new List<PreparationStep>
                {
                    new PreparationStep(
                        Identifier.NewId(),
                        new Text("Step 1")
                    )
                },
                new List<WeightedIngredient>
                {
                    new WeightedIngredient(
                        Identifier.NewId(),
                        Weight.FromGram(100),
                        new Text("Eier")
                    )
                },
                new Image()
            ),
            new Recipe(
                new Identifier(Guid.NewGuid()),
                new Username("admin"),
                new Text("This is a recipe"),
                new Text("This is a description"),
                Visibility.PUBLIC,
                new Portion(new Rational<int>(1,1)),
                new Duration(TimeSpan.FromMinutes(20)),
                new List<Tag> {
                    new Tag("Tag 1"),
                    new Tag("Tag 2")
                },
                new List<PreparationStep>
                {
                    new PreparationStep(
                        Identifier.NewId(),
                        new Text("Step 1")
                    )
                },
                new List<WeightedIngredient>
                {
                    new WeightedIngredient(
                        Identifier.NewId(),
                        Weight.FromGram(100),
                        new Text("Eier")
                    )
                },
                new Image()
            ),
            new Recipe(
                new Identifier(Guid.NewGuid()),
                new Username("admin"),
                new Text("This is a recipe ahegpia egpoha iog haehg long title paeiobghpoiah gieoahgpo iihgahopie ghpoia hgpioe hagpioeahg ea hgeaiog hapoig hopiaeg hp"),
                new Text("This is a description"),
                Visibility.PUBLIC,
                new Portion(new Rational<int>(1,1)),
                new Duration(TimeSpan.FromMinutes(20)),
                new List<Tag> {
                    new Tag("Tag 1"),
                    new Tag("Tag 2")
                },
                new List<PreparationStep>
                {
                    new PreparationStep(
                        Identifier.NewId(),
                        new Text("Step 1")
                    )
                },
                new List<WeightedIngredient>
                {
                    new WeightedIngredient(
                        Identifier.NewId(),
                        Weight.FromGram(100),
                        new Text("Eier")
                    )
                },
                new Image()
            ),
            new Recipe(
                new Identifier(Guid.NewGuid()),
                new Username("admin"),
                new Text("This is a recipe"),
                new Text("This is a description"),
                Visibility.PUBLIC,
                new Portion(new Rational<int>(1,1)),
                new Duration(TimeSpan.FromMinutes(20)),
                new List<Tag> {
                    new Tag("Tag 1"),
                    new Tag("Tag 2")
                },
                new List<PreparationStep>
                {
                    new PreparationStep(
                        Identifier.NewId(),
                        new Text("Step 1")
                    )
                },
                new List<WeightedIngredient>
                {
                    new WeightedIngredient(
                        Identifier.NewId(),
                        Weight.FromGram(100),
                        new Text("Eier")
                    )
                },
                new Image()
            ),
            new Recipe(
                new Identifier(Guid.NewGuid()),
                new Username("admin"),
                new Text("This is a recipe ahegpia egpoha iog haehg long title paeiobghpoiah gieoahgpo iihgahopie ghpoia hgpioe hagpioeahg ea hgeaiog hapoig hopiaeg hp"),
                new Text("This is a description"),
                Visibility.PUBLIC,
                new Portion(new Rational<int>(1,1)),
                new Duration(TimeSpan.FromMinutes(20)),
                new List<Tag> {
                    new Tag("Tag 1"),
                    new Tag("Tag 2")
                },
                new List<PreparationStep>
                {
                    new PreparationStep(
                        Identifier.NewId(),
                        new Text("Step 1")
                    )
                },
                new List<WeightedIngredient>
                {
                    new WeightedIngredient(
                        Identifier.NewId(),
                        Weight.FromGram(100),
                        new Text("Eier")
                    )
                },
                new Image()
            ),
            new Recipe(
                new Identifier(Guid.NewGuid()),
                new Username("admin"),
                new Text("This is a recipe"),
                new Text("This is a description"),
                Visibility.PUBLIC,
                new Portion(new Rational<int>(1,1)),
                new Duration(TimeSpan.FromMinutes(20)),
                new List<Tag> {
                    new Tag("Tag 1"),
                    new Tag("Tag 2")
                },
                new List<PreparationStep>
                {
                    new PreparationStep(
                        Identifier.NewId(),
                        new Text("Step 1")
                    )
                },
                new List<WeightedIngredient>
                {
                    new WeightedIngredient(
                        Identifier.NewId(),
                        Weight.FromGram(100),
                        new Text("Eier")
                    )
                },
                new Image()
            ),
            new Recipe(
                new Identifier(Guid.NewGuid()),
                new Username("admin"),
                new Text("This is a recipe ahegpia egpoha iog haehg long title paeiobghpoiah gieoahgpo iihgahopie ghpoia hgpioe hagpioeahg ea hgeaiog hapoig hopiaeg hp"),
                new Text("This is a description"),
                Visibility.PUBLIC,
                new Portion(new Rational<int>(1,1)),
                new Duration(TimeSpan.FromMinutes(20)),
                new List<Tag> {
                    new Tag("Tag 1"),
                    new Tag("Tag 2")
                },
                new List<PreparationStep>
                {
                    new PreparationStep(
                        Identifier.NewId(),
                        new Text("Step 1")
                    )
                },
                new List<WeightedIngredient>
                {
                    new WeightedIngredient(
                        Identifier.NewId(),
                        Weight.FromGram(100),
                        new Text("Eier")
                    )
                },
                new Image()
            ),
            new Recipe(
                new Identifier(Guid.NewGuid()),
                new Username("admin"),
                new Text("This is a recipe"),
                new Text("This is a description"),
                Visibility.PUBLIC,
                new Portion(new Rational<int>(1,1)),
                new Duration(TimeSpan.FromMinutes(20)),
                new List<Tag> {
                    new Tag("Tag 1"),
                    new Tag("Tag 2")
                },
                new List<PreparationStep>
                {
                    new PreparationStep(
                        Identifier.NewId(),
                        new Text("Step 1")
                    )
                },
                new List<WeightedIngredient>
                {
                    new WeightedIngredient(
                        Identifier.NewId(),
                        Weight.FromGram(100),
                        new Text("Eier")
                    )
                },
                new Image()
            ),
            new Recipe(
                new Identifier(Guid.NewGuid()),
                new Username("admin"),
                new Text("This is a recipe ahegpia egpoha iog haehg long title paeiobghpoiah gieoahgpo iihgahopie ghpoia hgpioe hagpioeahg ea hgeaiog hapoig hopiaeg hp"),
                new Text("This is a description"),
                Visibility.PUBLIC,
                new Portion(new Rational<int>(1,1)),
                new Duration(TimeSpan.FromMinutes(20)),
                new List<Tag> {
                    new Tag("Tag 1"),
                    new Tag("Tag 2")
                },
                new List<PreparationStep>
                {
                    new PreparationStep(
                        Identifier.NewId(),
                        new Text("Step 1")
                    )
                },
                new List<WeightedIngredient>
                {
                    new WeightedIngredient(
                        Identifier.NewId(),
                        Weight.FromGram(100),
                        new Text("Eier")
                    )
                },
                new Image()
            ),
            new Recipe(
                new Identifier(Guid.NewGuid()),
                new Username("admin"),
                new Text("This is a recipe"),
                new Text("This is a description"),
                Visibility.PUBLIC,
                new Portion(new Rational<int>(1,1)),
                new Duration(TimeSpan.FromMinutes(20)),
                new List<Tag> {
                    new Tag("Tag 1"),
                    new Tag("Tag 2")
                },
                new List<PreparationStep>
                {
                    new PreparationStep(
                        Identifier.NewId(),
                        new Text("Step 1")
                    )
                },
                new List<WeightedIngredient>
                {
                    new WeightedIngredient(
                        Identifier.NewId(),
                        Weight.FromGram(100),
                        new Text("Eier")
                    )
                },
                new Image()
            ),
            new Recipe(
                new Identifier(Guid.NewGuid()),
                new Username("admin"),
                new Text("This is a recipe ahegpia egpoha iog haehg long title paeiobghpoiah gieoahgpo iihgahopie ghpoia hgpioe hagpioeahg ea hgeaiog hapoig hopiaeg hp"),
                new Text("This is a description"),
                Visibility.PUBLIC,
                new Portion(new Rational<int>(1,1)),
                new Duration(TimeSpan.FromMinutes(20)),
                new List<Tag> {
                    new Tag("Tag 1"),
                    new Tag("Tag 2")
                },
                new List<PreparationStep>
                {
                    new PreparationStep(
                        Identifier.NewId(),
                        new Text("Step 1")
                    )
                },
                new List<WeightedIngredient>
                {
                    new WeightedIngredient(
                        Identifier.NewId(),
                        Weight.FromGram(100),
                        new Text("Eier")
                    )
                },
                new Image()
            ),
        };

        using var home = resourceLoader.LoadResource("home.html")!;
        var homeContent = new StreamReader(home).ReadToEnd();
        var homeTemplate = Template.Parse(homeContent);

        Chef? currentChef = null;
        if (request.Cookies["session"] is Cookie cookie && !cookie.Expired && cookie.Value is string sessionId)
        {
            currentChef = sessionService.GetBySessionId(Identifier.Parse(sessionId));
        }

        return await homeTemplate.RenderAsync(new
        {
            Header = await new Header(resourceLoader).RenderAsync(currentChef),
            RecipeList = await new RecipeList(resourceLoader).RenderAsync(recipes)
        });
    }
}
