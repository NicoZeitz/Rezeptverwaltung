using System.Collections.Immutable;
using System.Net;

namespace Server.ContentParser;

internal class EmptyContentParser : IContentParser
{
    public bool CanParse(HttpListenerRequest request) => false;
    public IDictionary<string, ContentData> ParseRequest(HttpListenerRequest request) => ImmutableDictionary<string, ContentData>.Empty;
}