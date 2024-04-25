using Core.Entities;
using Core.Interfaces;
using Core.Repository;
using Core.Services;
using Core.Services.Password;
using Core.Services.Serialization;
using Core.ValueObjects;
using Database;
using Database.Repositories;
using FileSystem;
using Logging;
using Main;
using Microsoft.Extensions.DependencyInjection;
using Server;
using Server.Components;
using Server.ContentParser;
using Server.DataParser;
using Server.PageRenderer;
using Server.RequestHandler;
using Server.ResourceLoader;
using Server.Resources;
using Server.Service;
using Server.Session;

var configuration = new ApplicationConfiguration();
var provider = configureServices(configuration);
var serverCancellationToken = new CancellationToken();
var server = provider.GetRequiredService<Server.Server>();
configureServerRoutes(server);
server.Run(serverCancellationToken).GetAwaiter().GetResult();

IServiceProvider configureServices(ApplicationConfiguration configuration)
{
    var services = new ServiceCollection();

    // Configuration
    services.AddSingleton<DatabaseConfiguration>(configuration);
    services.AddSingleton<ServerConfiguration>(configuration);

    // Logger
    services.AddSingleton<Logger>(provider => createLogger(provider));

    // Database
    // This is very ugly but we wanted to try out what it means to use a
    // hardcoded singleton anyways 😉
    services.AddSingleton(provider => Database.Database.Instance.Initialize(
        provider.GetRequiredService<DatabaseConfiguration>(),
        provider.GetRequiredService<Logger>()
    ));

    // Server
    services.AddSingleton<Server.Server>();
    services.AddSingleton<ComponentProvider>(p => new UIComponentProvider(p));

    // File System
    services.AddSingleton(new FileSystem.FileSystem(configuration.ApplicationDirectory.Join(new Core.ValueObjects.Directory("images"))));

    // Repositories
    services.AddTransient<ChefRepository, ChefDatabase>();
    services.AddTransient<CookbookRepository, CookbookDatabase>();
    services.AddTransient<RecipeRepository, RecipeDatabase>();
    services.AddTransient<ShoppingListRepository, ShoppingListDatabase>();

    // Password Services
    var allowedPasswordChecker = new AllowedPasswordChecker();
    allowedPasswordChecker.AddPasswordConditionChecker(new PasswordLengthChecker(8));
    allowedPasswordChecker.AddPasswordConditionChecker(new PasswordUppercaseChecker());
    allowedPasswordChecker.AddPasswordConditionChecker(new PasswordLowercaseChecker());
    allowedPasswordChecker.AddPasswordConditionChecker(new PasswordDigitChecker());
    allowedPasswordChecker.AddPasswordConditionChecker(new PasswordSpecialCharacterChecker());
    services.AddSingleton(allowedPasswordChecker);
    services.AddTransient<DuplicatePasswordChecker>();
    services.AddTransient<PasswordHasher, Argon2PasswordHasher>();

    // Services
    services.AddTransient<ChangeChefDataService>();
    services.AddTransient<ChangeChefPasswordService>();
    services.AddTransient<CreateCookbookService>();
    services.AddTransient<CreateRecipeService>();
    services.AddTransient<CreateShoppingListService>();
    services.AddTransient<DeleteChefService>();
    services.AddTransient<DeleteCookbookService>();
    services.AddTransient<DeleteRecipeService>();
    services.AddTransient<DeleteShoppingListService>();
    services.AddTransient<ImageService, FileSystemImageService>();
    services.AddTransient<LoginChefService>();
    services.AddTransient<MeasurementUnitCombiner>();
    services.AddTransient<RegisterChefService>();
    services.AddTransient<ShoppingListEntriesCreator>();
    services.AddTransient<ShowChefs>();
    services.AddTransient<ShowCookbooks>();
    services.AddTransient<ShowRecipes>();
    services.AddTransient<ShowShoppingLists>();
    var measurementUnitSerializationManager = new MeasurementUnitSerializationManager();
    measurementUnitSerializationManager.RegisterSerializer(new CupSerializer());
    measurementUnitSerializationManager.RegisterSerializer(new PieceSerializer());
    measurementUnitSerializationManager.RegisterSerializer(new PinchSerializer());
    measurementUnitSerializationManager.RegisterSerializer(new SpoonSerializer());
    measurementUnitSerializationManager.RegisterSerializer(new VolumeSerializer());
    measurementUnitSerializationManager.RegisterSerializer(new WeightSerializer());
    services.AddSingleton(measurementUnitSerializationManager);

    // Database Services
    services.AddTransient<DateTimeProvider, DefaultDateTimeProvider>();
    services.AddTransient<ParameterNameGenerator>();

    // Server Services
    services.AddSingleton<SessionBackend<Chef>, InMemorySessionBackend<Chef>>();
    services.AddTransient<ContentParserFactory>();
    services.AddTransient<CookbookPostDataParser>();
    services.AddTransient<HTMLFileWriter>();
    services.AddTransient<HTMLSanitizer>();
    services.AddTransient<ImageTypeMimeTypeConverter>();
    services.AddTransient<ImageUrlService>();
    services.AddTransient<LoginPageRenderer>();
    services.AddTransient<LoginPostDataParser>();
    services.AddTransient<MimeTypeDeterminer>();
    services.AddTransient<NewCookbookPageRenderer>();
    services.AddTransient<NewRecipePageRenderer>();
    services.AddTransient<NewShoppingListPageRenderer>();
    services.AddTransient<NotFoundPageRenderer>();
    services.AddTransient<RecipePostDataParser>();
    services.AddTransient<RedirectService>();
    services.AddTransient<RegisterPageRenderer>();
    services.AddTransient<RegisterPostDataParser>();
    services.AddTransient<SessionService, CookieSessionService>();
    services.AddTransient<SettingsPageRenderer>();
    services.AddTransient<SettingsPostDataParser>();
    services.AddTransient<ShoppingListPostDataParser>();
    services.AddTransient<URLEncoder>();

    // Resource Loader
#if DEBUG
    services.AddSingleton<ResourceLoader>(provider => new FileSystemResourceLoader(
        new Core.ValueObjects.Directory(Path.GetFullPath("..\\..\\..\\..\\Server\\Components\\template\\")),
        provider.GetRequiredService<Logger>()
    ));
#else
    services.AddSingleton<ResourceLoader, EmbeddedResourceLoader>();
#endif
    services.AddKeyedSingleton<ResourceLoader>("ASSETS", (provider, _) => new PrefixResourceLoader(
        "assets",
        provider.GetRequiredService<ResourceLoader>()
    ));
    services.AddTransient<TemplateLoader>();

    // Components
    services.AddTransient<CookbookList>();
    services.AddTransient<Header>();
    services.AddTransient<RecipeList>();
    services.AddTransient<ShoppingListList>();

    // Pages
    services.AddTransient<ChefDetailPage>();
    services.AddTransient<CookbookDetailPage>();
    services.AddTransient<HomePage>();
    services.AddTransient<LoginPage>();
    services.AddTransient<NewCookbookPage>();
    services.AddTransient<NewCookbookPage>();
    services.AddTransient<NewRecipePage>();
    services.AddTransient<NewShoppingListPage>();
    services.AddTransient<NotFoundPage>();
    services.AddTransient<RecipeDetailPage>();
    services.AddTransient<RegisterPage>();
    services.AddTransient<SearchPage>();
    services.AddTransient<SettingsPage>();
    services.AddTransient<ShoppingListDetailPage>();
    services.AddTransient<TagPage>();

    // Request Handlers
    services.AddTransient<FaviconRequestHandler>(provider => new FaviconRequestHandler(provider.GetRequiredKeyedService<ResourceLoader>("ASSETS")));
    services.AddTransient<ChefDetailRequestHandler>();
    services.AddTransient<CookbookDetailRequestHandler>();
    services.AddTransient<DeleteCookbookRequestHandler>();
    services.AddTransient<DeleteRecipeRequestHandler>();
    services.AddTransient<DeleteShoppingListRequestHandler>();
    services.AddTransient<HomeRequestHandler>();
    services.AddTransient<ImageRequestHandler>();
    services.AddTransient<LoginRequestHandler>();
    services.AddTransient<LogoutRequestHandler>();
    services.AddTransient<NewCookbookPage>();
    services.AddTransient<NewCookbookRequestHandler>();
    services.AddTransient<NewRecipeRequestHandler>();
    services.AddTransient<NewShoppingListRequestHandler>();
    services.AddTransient<NotFoundRequestHandler>();
    services.AddTransient<RecipeDetailRequestHandler>();
    services.AddTransient<RegisterRequestHandler>();
    services.AddTransient<SearchRequestHandler>();
    services.AddTransient<SettingsRequestHandler>();
    services.AddTransient<ShoppingListDetailRequestHandler>();
    services.AddTransient<TagRequestHandler>();
    services.AddTransient<StaticRequestHandler>(provider => new StaticRequestHandler(
        "assets/",
        provider.GetRequiredKeyedService<ResourceLoader>("ASSETS"),
        provider.GetRequiredService<MimeTypeDeterminer>()
    ));

    return services.BuildServiceProvider();
}

void configureServerRoutes(Server.Server server)
{
    server.AddRequestHandler(provider.GetRequiredService<SearchRequestHandler>());
    server.AddRequestHandler(provider.GetRequiredService<HomeRequestHandler>());
    server.AddRequestHandler(provider.GetRequiredService<FaviconRequestHandler>());
    server.AddRequestHandler(provider.GetRequiredService<ChefDetailRequestHandler>());
    server.AddRequestHandler(provider.GetRequiredService<CookbookDetailRequestHandler>());
    server.AddRequestHandler(provider.GetRequiredService<DeleteCookbookRequestHandler>());
    server.AddRequestHandler(provider.GetRequiredService<DeleteRecipeRequestHandler>());
    server.AddRequestHandler(provider.GetRequiredService<DeleteShoppingListRequestHandler>());
    server.AddRequestHandler(provider.GetRequiredService<LoginRequestHandler>());
    server.AddRequestHandler(provider.GetRequiredService<LogoutRequestHandler>());
    server.AddRequestHandler(provider.GetRequiredService<NewCookbookRequestHandler>());
    server.AddRequestHandler(provider.GetRequiredService<NewCookbookRequestHandler>());
    server.AddRequestHandler(provider.GetRequiredService<NewRecipeRequestHandler>());
    server.AddRequestHandler(provider.GetRequiredService<NewShoppingListRequestHandler>());
    server.AddRequestHandler(provider.GetRequiredService<RecipeDetailRequestHandler>());
    server.AddRequestHandler(provider.GetRequiredService<RegisterRequestHandler>());
    server.AddRequestHandler(provider.GetRequiredService<SettingsRequestHandler>());
    server.AddRequestHandler(provider.GetRequiredService<ShoppingListDetailRequestHandler>());
    server.AddRequestHandler(provider.GetRequiredService<TagRequestHandler>());
    server.AddRequestHandler(provider.GetRequiredService<ImageRequestHandler>());
    server.AddRequestHandler(provider.GetRequiredService<StaticRequestHandler>());
    server.AddRequestHandler(provider.GetRequiredService<NotFoundRequestHandler>());
}

Logger createLogger(IServiceProvider provider) => new LogLevelLogger(
    new DateTimeLogger(
        new SplitLogger(
            new LogLevelFilter(
                new ColorLogger(
                    new ConsoleLogger()
                ),
                LogLevel.Info
            ),
            new LogLevelFilter(
                new FileLogger(
                    new Core.ValueObjects.File(
                        configuration.ApplicationDirectory.Join(new Core.ValueObjects.Directory("logs")),
                        FileName.From("rezeptverwaltung.log")
                    )
                ),
                LogLevel.Trace
            )
        ),
        provider.GetRequiredService<DateTimeProvider>()
    )
);

internal record UIComponentProvider(IServiceProvider ServiceProvider) : ComponentProvider
{
    public T GetComponent<T>() where T : Component => ServiceProvider.GetRequiredService<T>();
}
