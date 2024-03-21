using Core.Entities;
using Core.Services;
using Core.Services.Password;
using Database.Repositories;
using Server.RequestHandler;
using Server.ResourceLoader;
using Server.Session;
using System.Net;

//Console.WriteLine(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location));
//string[] resNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();
//foreach (string resName in resNames)
//    Console.WriteLine(resName);

Database.Database.Instance.Initialize();

var serverIp = IPAddress.Parse("127.0.0.1");
var serverPort = 8080;
var serverCancellationToken = new CancellationToken();

var server = new Server.Server(serverIp, serverPort);
configureServerRoutes(server);
server.Run(serverCancellationToken).GetAwaiter().GetResult();

void configureServerRoutes(Server.Server server)
{
    //var embeddedResourceLoader = new EmbeddedResourceLoader();
    var embeddedResourceLoader = new FileSystemResourceLoader("C:\\Users\\nicoz\\Dev\\CSharp\\Rezeptverwaltung\\Rezeptverwaltung\\Server\\template\\");
    var assetResourceLoader = new PrefixResourceLoader("assets", embeddedResourceLoader);
    //var fileSystemResourceLoader = new FileSystemResourceLoader("static.web");
    //server.AddRequestHandler(new RegisterRequestHandler(embeddedResourceLoader));
    //server.AddRequestHandler(new LogoutRequestHandler(embeddedResourceLoader));
    // API
    var chefRepository = new ChefDatabase();
    var argonPasswordHasher = new Argon2PasswordHasher();
    var chefLoginService = new ChefLoginService(chefRepository, argonPasswordHasher);
    var sessionService = new InMemorySessionService<Chef>();

    server.AddRequestHandler(new HomeRequestHandler(embeddedResourceLoader, sessionService));
    server.AddRequestHandler(new LoginRequestHandler(chefLoginService, embeddedResourceLoader, sessionService));
    server.AddRequestHandler(new LogoutRequestHandler(sessionService));
    server.AddRequestHandler(new AssetRequestHandler(assetResourceLoader));
    server.AddRequestHandler(new NotFoundHandler(embeddedResourceLoader));
}









//var embeddedResourceLoader = new EmbeddedResourceLoader();
//var staticFileResourceLoader = new PrefixResourceLoader("static.web", embeddedResourceLoader);
//var staticFileRequestHandler = new StaticFileHandler(staticFileResourceLoader);

//var server = new Server.Server(IPAddress.Parse("127.0.0.1"), 8080, embeddedResourceLoader);

//server.AddRequestHandler(new HomeRequestHandler(new PrefixResourceLoader("template", embeddedResourceLoader)));

//server.AddRequestHandler(staticFileRequestHandler);
//server.Run(cancellationToken).GetAwaiter().GetResult();
