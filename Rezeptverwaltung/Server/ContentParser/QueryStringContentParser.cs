using System.Collections.Immutable;
using System.Net;
using System.Web;

namespace Server.ContentParser;

internal class QueryStringContentParser : ContentParser
{
    public bool CanParse(HttpListenerRequest request)
    {
        return IsQueryStringRequest(request);
    }

    public IDictionary<string, ContentData> ParseRequest(HttpListenerRequest request)
    {
        if (IsFormUrlEncodedRequest(request))
        {
            return ParseFormUrlEncodedRequest(request);
        }

        if (IsQueryStringEncodedRequest(request))
        {
            return ParseQueryStringEncodedRequest(request);
        }

        return ImmutableDictionary.Create<string, ContentData>();
    }

    public bool IsQueryStringRequest(HttpListenerRequest request)
    {
        return IsFormUrlEncodedRequest(request) || IsQueryStringEncodedRequest(request);
    }


    private IDictionary<string, ContentData> ParseFormUrlEncodedRequest(HttpListenerRequest request)
    {
        using var body = request.InputStream;
        using var reader = new StreamReader(body, request.ContentEncoding);
        var bodyString = reader.ReadToEnd();

        return ParseQueryValues(bodyString);
    }

    private IDictionary<string, ContentData> ParseQueryStringEncodedRequest(HttpListenerRequest request)
    {
        if(request.Url?.Query is null)
        {
            return ImmutableDictionary.Create<string, ContentData>();
        }

        return ParseQueryValues(request.Url.Query);
    }

    private IDictionary<string, ContentData> ParseQueryValues(string query)
    {
        var queryValues = HttpUtility.ParseQueryString(query);
        var parameterDataDictionary = new Dictionary<string, ContentData>();
        foreach (var parameter in queryValues.AllKeys)
        {
            if (parameter is not null && queryValues[parameter] is { } data)
            {
                parameterDataDictionary.Add(parameter, ContentData.FromString(data));
            }
        }

        return parameterDataDictionary;
    }

    private bool IsFormUrlEncodedRequest(HttpListenerRequest request)
    {
        return request.ContentType == "application/x-www-form-urlencoded" && request.HasEntityBody;
    }

    private bool IsQueryStringEncodedRequest(HttpListenerRequest request)
    {
        return request.Url?.Query is not null;
    }
}
