using System.Net;

namespace Server.ContentParser;

public interface IContentParser
{
    bool CanParse(HttpListenerRequest request);
    IDictionary<string, ContentData> ParseRequest(HttpListenerRequest request);
}
