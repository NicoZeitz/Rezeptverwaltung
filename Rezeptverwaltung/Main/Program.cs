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


//Console.WriteLine(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location));
//string[] resNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();
//foreach (string resName in resNames)
//    Console.WriteLine(resName);

var provider = configureServices();

var serverIp = IPAddress.Parse("127.0.0.1"); // TODO: is hard coded
var serverPort = 8080;
var serverCancellationToken = new CancellationToken();

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
    var resourceLoader = new FileSystemResourceLoader("C:\\Users\\nicoz\\Dev\\CSharp\\Rezeptverwaltung\\Rezeptverwaltung\\Server\\Components\\template\\");
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