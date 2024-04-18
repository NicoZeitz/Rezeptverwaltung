using Core.Entities;
using Core.Repository;
using Core.Services;
using Core.Services.Password;
using Database.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Server.Components;
using Server.RequestHandler;
using Server.RequestHandler.Chef;
using Server.RequestHandler.Recipe;
using Server.RequestHandler.Register;
using Server.ResourceLoader;
using Server.Session;
using System.Net;
using Core.ValueObjects;
using Core.ValueObjects.MeasurementUnits;


//Console.WriteLine(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location));
//string[] resNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();
//foreach (string resName in resNames)
//    Console.WriteLine(resName);

var provider = configureServices();

var serverIp = IPAddress.Parse("127.0.0.1"); // TODO: is hard coded
var serverPort = 8080;
var serverCancellationToken = new CancellationToken();

var chefRepository = provider.GetRequiredService<ChefRepository>();
var recipeRepository = provider.GetRequiredService<RecipeRepository>();

//chefRepository.Add(
//    new Chef(
//        new Username("MeisterkochFabian"),
//        new Name("Fabian", "Wolf"),
//        provider.GetRequiredService<PasswordHasher>().HashPassword(new Password("wolf"))
//    )
//);

recipeRepository.Add(
    new Recipe(
            Identifier.NewId(),
            new Username("MeisterkochFabian"),
            new Text("Nudelauflauf"),
            new Text("Nudelauflauf aus dem Hause Wolf"),
            Visibility.PUBLIC,
            new Portion(new Rational<int>(2, 1)),
            new Duration(TimeSpan.FromMinutes(50)),
            new List<Tag>
            {
                new Tag("Nudeln"),
                new Tag("Auflauf"),
                new Tag("Eier"),
                new Tag("Würstchen"),
                new Tag("Käse")
            },
            new List<PreparationStep>
            {
                new PreparationStep(new Text("Nudeln kochen")),
                new PreparationStep(new Text("Form einfetten und mit Semmelbröseln bestreuen")),
                new PreparationStep(new Text("Erste Hälfte der Nudeln in die Form")),
                new PreparationStep(new Text("Kleingeschnittene Würstchen und 2/3 des Käses in die Form")),
                new PreparationStep(new Text("Zweite Hälfte der Nudeln in die Form")),
                new PreparationStep(new Text("Mit Prise Salz gequirlte Eier über den Forminhalt gießen")),
                new PreparationStep(new Text("Reste des Käses mit Butterflocken und Paprikapulver darüber verteilen")),
                new PreparationStep(new Text("Im Ofen bei 180°C Umluft 30 Minuten backen")),
            },
            new List<WeightedIngredient>
            {
                new WeightedIngredient(
                    Weight.FromGram(160),
                    new Text("Nudeln")
                ),
                new WeightedIngredient(
                    new Piece(3),
                    new Text("Wiener Würstchen")
                ),
                new WeightedIngredient(
                    Weight.FromKilogram(150),
                    new Text("Gouda")
                ),
                new WeightedIngredient(
                    new Piece(2),
                    new Text("Eier")
                ),
                new WeightedIngredient(
                    new Pinch(1),
                    new Text("Salz")
                ),
                new WeightedIngredient(
                    new Spoon(1, SpoonSize.TEA),
                    new Text("Paprikapulver")
                ),
                new WeightedIngredient(
                    new Spoon(5, SpoonSize.TABLE),
                    new Text("Semmelbrösel")
                ),
                                new WeightedIngredient(
                    new Spoon(2, SpoonSize.TABLE),
                    new Text("Butter")
                ),
            }
        )
);

return;

var server = new Server.Server(serverIp, serverPort);

server.AddRequestHandler(provider.GetRequiredService<HomeRequestHandler>());
server.AddRequestHandler(provider.GetRequiredService<RegisterRequestHandler>());
server.AddRequestHandler(provider.GetRequiredService<RecipeDetailRequestHandler>());
server.AddRequestHandler(provider.GetRequiredService<ChefDetailRequestHandler>());
server.AddRequestHandler(provider.GetRequiredService<LoginRequestHandler>());
server.AddRequestHandler(provider.GetRequiredService<LogoutRequestHandler>());
server.AddRequestHandler(provider.GetRequiredService<StaticRequestHandler>());
server.AddRequestHandler(provider.GetRequiredService<NotFoundRequestHandler>());

server.Run(serverCancellationToken).GetAwaiter().GetResult();

IServiceProvider configureServices()
{
    var services = new ServiceCollection();
    services.AddSingleton(Database.Database.Instance);
    services.AddTransient<ChefRepository, ChefDatabase>();
    services.AddTransient<RecipeRepository, RecipeDatabase>();
    services.AddTransient<CookbookRepository, CookbookDatabase>();
    services.AddTransient<ShoppingListRepository, ShoppingListDatabase>();

    services.AddTransient<MeasurementUnitSerializationManager>();
    services.AddTransient<DateTimeProvider, DefaultDateTimeProvider>();
    services.AddTransient<PasswordHasher, Argon2PasswordHasher>();
    services.AddTransient<AllowedPasswordChecker>();
    services.AddTransient<DuplicatePasswordChecker>();
    services.AddTransient<ShowRecipes>();

    services.AddTransient<ChefLoginService>();
    services.AddTransient<ChefRegisterService>();

    // var resourceLoader = new EmbeddedResourceLoader();
    var resourceLoader = new FileSystemResourceLoader("C:\\SAPDevelop\\DHBW\\Rezeptverwaltung\\Rezeptverwaltung\\Server\\Components\\template\\");
    var assetResourceLoader = new PrefixResourceLoader("assets", resourceLoader);

    services.AddSingleton<ResourceLoader>(resourceLoader);
    services.AddKeyedSingleton<ResourceLoader>("ASSETS", assetResourceLoader);

    // Components
    services.AddTransient<Header>();
    services.AddTransient<RecipeList>();

    // Pages
    services.AddTransient<ChefDetailPage>();
    services.AddTransient<CookbookDetailPage>();
    services.AddTransient<HomePage>();
    services.AddTransient<LoginPage>();
    services.AddTransient<NewCookbookPage>();
    services.AddTransient<NewRecipePage>();
    services.AddTransient<NewShoppingListPage>();
    services.AddTransient<NotFoundPage>();
    services.AddTransient<RecipeDetailPage>();
    services.AddTransient<RegisterPage>();
    services.AddTransient<SettingsPage>();
    services.AddTransient<ShoppingListDetailPage>();

    services.AddTransient<HomeRequestHandler>();
    services.AddTransient<RegisterRequestHandler>();
    services.AddTransient<ChefDetailRequestHandler>();
    services.AddTransient<LoginRequestHandler>();
    services.AddTransient<LogoutRequestHandler>();
    services.AddTransient<NotFoundRequestHandler>();
    services.AddTransient<RecipeDetailRequestHandler>();
    services.AddTransient<StaticRequestHandler>(provider =>
        new StaticRequestHandler("assets/", provider.GetRequiredKeyedService<ResourceLoader>("ASSETS")));

    services.AddSingleton<SessionBackend<Chef>, InMemorySessionBackend<Chef>>();
    services.AddTransient<SessionService, CookieSessionService>();


    return services.BuildServiceProvider();
}