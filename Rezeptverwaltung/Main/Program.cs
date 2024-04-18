using Core.Entities;
using Core.Repository;
using Core.Services;
using Core.Services.Password;
using Main;
using Microsoft.Extensions.DependencyInjection;
using Persistence.DB;
using Persistence.FS;
using Persistence.Repositories;
using Server;
using Server.Components;
using Server.ContentParser;
using Server.RequestHandler;
using Server.RequestHandler.Chef;
using Server.RequestHandler.Login;
using Server.RequestHandler.Recipe;
using Server.RequestHandler.Register;
using Server.ResourceLoader;
using Server.Resources;
using Server.Session;
using System.Reflection;


//Console.WriteLine(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location));
//string[] resNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();
//foreach (string resName in resNames)
//    Console.WriteLine(resName);

var configuration = new ApplicationConfiguration();
var provider = configureServices(configuration);
var serverCancellationToken = new CancellationToken();
var server = provider.GetRequiredService<Server.Server>();

server.AddRequestHandler(provider.GetRequiredService<HomeRequestHandler>());
server.AddRequestHandler(provider.GetRequiredService<RegisterRequestHandler>());
server.AddRequestHandler(provider.GetRequiredService<RecipeDetailRequestHandler>());
server.AddRequestHandler(provider.GetRequiredService<ChefDetailRequestHandler>());
server.AddRequestHandler(provider.GetRequiredService<LoginRequestHandler>());
server.AddRequestHandler(provider.GetRequiredService<LogoutRequestHandler>());
server.AddRequestHandler(provider.GetRequiredService<StaticRequestHandler>());
server.AddRequestHandler(provider.GetRequiredService<NotFoundRequestHandler>());


var chefRepository = provider.GetRequiredService<ChefRepository>();
var recipeRepository = provider.GetRequiredService<RecipeRepository>();

//chefRepository.Add(
//    new Chef(
//        new Username("MeisterkochFabian"),
//        new Name("Fabian", "Wolf"),
//        provider.GetRequiredService<PasswordHasher>().HashPassword(new Password("wolf"))
//    )
//);

//recipeRepository.Add(
//    new Recipe(
//            Identifier.NewId(),
//            new Username("MeisterkochFabian"),
//            new Text("Nudelauflauf"),
//            new Text("Nudelauflauf aus dem Hause Wolf"),
//            Visibility.PUBLIC,
//            new Portion(new Rational<int>(2, 1)),
//            new Duration(TimeSpan.FromMinutes(50)),
//            new List<Tag>
//            {
//                new Tag("Nudeln"),
//                new Tag("Auflauf"),
//                new Tag("Eier"),
//                new Tag("Würstchen"),
//                new Tag("Käse")
//            },
//            new List<PreparationStep>
//            {
//                new PreparationStep(new Text("Nudeln kochen")),
//                new PreparationStep(new Text("Form einfetten und mit Semmelbröseln bestreuen")),
//                new PreparationStep(new Text("Erste Hälfte der Nudeln in die Form")),
//                new PreparationStep(new Text("Kleingeschnittene Würstchen und 2/3 des Käses in die Form")),
//                new PreparationStep(new Text("Zweite Hälfte der Nudeln in die Form")),
//                new PreparationStep(new Text("Mit Prise Salz gequirlte Eier über den Forminhalt gießen")),
//                new PreparationStep(new Text("Reste des Käses mit Butterflocken und Paprikapulver darüber verteilen")),
//                new PreparationStep(new Text("Im Ofen bei 180°C Umluft 30 Minuten backen")),
//            },
//            new List<WeightedIngredient>
//            {
//                new WeightedIngredient(
//                    Weight.FromGram(160),
//                    new Text("Nudeln")
//                ),
//                new WeightedIngredient(
//                    new Piece(3),
//                    new Text("Wiener Würstchen")
//                ),
//                new WeightedIngredient(
//                    Weight.FromKilogram(150),
//                    new Text("Gouda")
//                ),
//                new WeightedIngredient(
//                    new Piece(2),
//                    new Text("Eier")
//                ),
//                new WeightedIngredient(
//                    new Pinch(1),
//                    new Text("Salz")
//                ),
//                new WeightedIngredient(
//                    new Spoon(1, SpoonSize.TEA),
//                    new Text("Paprikapulver")
//                ),
//                new WeightedIngredient(
//                    new Spoon(5, SpoonSize.TABLE),
//                    new Text("Semmelbrösel")
//                ),
//                                new WeightedIngredient(
//                    new Spoon(2, SpoonSize.TABLE),
//                    new Text("Butter")
//                ),
//            }
//        )
//);

server.Run(serverCancellationToken).GetAwaiter().GetResult();

IServiceProvider configureServices(ApplicationConfiguration configuration)
{
    var services = new ServiceCollection();

    // Configuration
    services.AddSingleton<DatabaseConfiguration>(configuration);
    services.AddSingleton<ServerConfiguration>(configuration);

    // Database
    services.AddSingleton(provider => Database.Instance.Initialize(provider.GetRequiredService<DatabaseConfiguration>()));

    // Server
    services.AddSingleton<Server.Server>();

    // File System
    services.AddSingleton<FileSystem>();

    // Repositories
    services.AddTransient<ChefRepository, ChefDatabase>();
    services.AddTransient<RecipeRepository, RecipeDatabase>();
    services.AddTransient<CookbookRepository, CookbookDatabase>();
    services.AddTransient<ShoppingListRepository, ShoppingListDatabase>();

    // Password Services
    services.AddTransient<AllowedPasswordChecker>();
    services.AddTransient<PasswordHasher, Argon2PasswordHasher>();
    services.AddTransient<DuplicatePasswordChecker>();

    // Services
    services.AddTransient<ChefLoginService>();
    services.AddTransient<ChefRegisterService>();
    services.AddTransient<ImageService, FileSystemImageService>();
    services.AddTransient<MeasurementUnitCombiner>();
    services.AddTransient<MeasurementUnitSerializationManager>();
    services.AddTransient<ShoppingListEntriesCreator>();
    services.AddTransient<ShowRecipes>();

    // Database Services
    services.AddTransient<ParameterNameGenerator>();
    services.AddTransient<DateTimeProvider, DefaultDateTimeProvider>();

    // Server Services
    services.AddTransient<ContentParserFactory>();
    services.AddSingleton<SessionBackend<Chef>, InMemorySessionBackend<Chef>>();
    services.AddTransient<SessionService, CookieSessionService>();
    services.AddTransient<ImageUrlService>();
    services.AddTransient<MimeTypeDeterminer>();
    services.AddTransient<MimeTypeToImageType>();
    services.AddTransient<LoginPageRenderer>();
    services.AddTransient<LoginPostDataParser>();
    services.AddTransient<RegisterPageRenderer>();
    services.AddTransient<RegisterPostDataParser>();

    // print current path to console
    Console.WriteLine(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location));

    // Resource Loader
    // var resourceLoader = new EmbeddedResourceLoader();
    Console.WriteLine(Path.GetFullPath("..\\..\\..\\..\\Server\\Components\\template\\"));

    var resourceLoader = new FileSystemResourceLoader(Path.GetFullPath("..\\..\\..\\..\\Server\\Components\\template\\"));
    var assetResourceLoader = new PrefixResourceLoader("assets", resourceLoader);
    services.AddSingleton<ResourceLoader>(resourceLoader);
    services.AddKeyedSingleton<ResourceLoader>("ASSETS", assetResourceLoader);
    services.AddTransient<TemplateLoader>();

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

    // Request Handlers
    services.AddTransient<HomeRequestHandler>();
    services.AddTransient<LoginRequestHandler>();
    services.AddTransient<LogoutRequestHandler>();
    services.AddTransient<NotFoundRequestHandler>();
    services.AddTransient<ChefDetailRequestHandler>();
    services.AddTransient<RecipeDetailRequestHandler>();
    services.AddTransient<RegisterRequestHandler>();
    services.AddTransient<StaticRequestHandler>(provider => new StaticRequestHandler(
        "assets/",
        provider.GetRequiredKeyedService<ResourceLoader>("ASSETS"),
        provider.GetRequiredService<MimeTypeDeterminer>()
    ));




    return services.BuildServiceProvider();
}