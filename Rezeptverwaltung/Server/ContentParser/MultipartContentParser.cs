using HttpMultipartParser;
using Server.ValueObjects;
using System.Collections.Immutable;
using System.Net;

namespace Server.ContentParser;

internal class MultipartContentParser : ContentParser
{
    public bool CanParse(HttpListenerRequest request)
    {
        return IsMultipartRequest(request);
    }

    public IDictionary<string, ContentData> ParseRequest(HttpListenerRequest request)
    {
        if (!IsMultipartRequest(request))
        {
            return ImmutableDictionary.Create<string, ContentData>();
        }

        var multipartFormDataParser = MultipartFormDataParser.Parse(request.InputStream, request.ContentEncoding);

        if (!EnsureParametersAreUnique(multipartFormDataParser))
        {
            return ImmutableDictionary.Create<string, ContentData>();
        }

        var parameterDataDictionary = new Dictionary<string, ContentData>();
        foreach (var parameter in multipartFormDataParser.Parameters)
        {
            parameterDataDictionary.Add(parameter.Name, ContentData.FromString(parameter.Data));
        }

        foreach (var file in multipartFormDataParser.Files)
        {
            var mimeType = MimeType.FromString(file.ContentType);
            if (!mimeType.HasValue)
            {
                continue;
            }

            parameterDataDictionary.Add(file.Name, ContentData.FromFile(file.Data, file.FileName, mimeType.Value));
        }

        return parameterDataDictionary;
    }

    public bool IsMultipartRequest(HttpListenerRequest request)
    {
        return request.ContentType != null && request.ContentType.StartsWith("multipart/form-data");
    }

    private bool EnsureParametersAreUnique(MultipartFormDataParser multipartFormDataParser)
    {
        var parameterNames = multipartFormDataParser.Parameters.Select(p => p.Name);
        var fileNames = multipartFormDataParser.Files.Select(f => f.Name);

        return parameterNames
            .Concat(fileNames)
            .GroupBy(p => p)
            .All(g => g.Count() == 1);
    }
}
