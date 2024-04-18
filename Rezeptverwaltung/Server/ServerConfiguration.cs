using System.Net;

namespace Server;

public interface ServerConfiguration
{
    public IPAddress IPAddress { get; }
    public int Port { get; }
}