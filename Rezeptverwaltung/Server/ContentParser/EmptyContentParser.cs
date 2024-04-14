using System.Collections.Immutable;
using System.Net;

namespace Server.ContentParser;

internal class EmptyContentParser : ContentParser
{
    public bool CanParse(HttpListenerRequest request) => false;
    public IDictionary<string, ContentData> ParseRequest(HttpListenerRequest request) => ImmutableDictionary<string, ContentData>.Empty;
}