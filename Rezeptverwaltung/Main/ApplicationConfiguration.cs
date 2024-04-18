using Persistence.DB;
using Server;
using System.Net;

namespace Main;

// TODO: Interface Segregation
public class ApplicationConfiguration : DatabaseConfiguration, ServerConfiguration
{
    public string DatabaseLocation => "rezeptverwaltung.db";

    public IPAddress IPAddress => IPAddress.Parse("127.0.0.1");

    public int Port => 8080;
}
