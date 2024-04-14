using System.Net;

namespace Server.ContentParser;

public interface ContentParser
{
    bool CanParse(HttpListenerRequest request);
    IDictionary<string, ContentData> ParseRequest(HttpListenerRequest request);
}
