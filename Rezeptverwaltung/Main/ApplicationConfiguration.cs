using Database;
using Server;
using System.Net;

namespace Main;

// TODO: Interface Segregation
public class ApplicationConfiguration : DatabaseConfiguration, ServerConfiguration
{
    public Core.ValueObjects.Directory ApplicationDirectory;
    public IPAddress IPAddress => IPAddress.Parse("127.0.0.1");
    public int Port => 8080;
    public Core.ValueObjects.File DatabaseLocation => Core.ValueObjects.File.From(ApplicationDirectory, "rezeptverwaltung.db");

    public ApplicationConfiguration()
    {
#if DEBUG
        ApplicationDirectory = new Core.ValueObjects.Directory(
            Path.GetFullPath("..\\..\\..\\..\\Rezeptverwaltung")
        );
#else
        ApplicationDirectory = new Core.ValueObjects.Directory(
            Path.GetFullPath(System.Reflection.Assembly.GetExecutingAssembly().Location ?? ".")
        );
#endif
    }
}
