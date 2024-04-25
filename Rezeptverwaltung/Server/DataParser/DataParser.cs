using Core.Data;
using Server.ContentParser;
using Server.Service;
using System.Net;

namespace Server.DataParser;

public abstract class DataParser<T>
{
    protected readonly Result<T> GENERIC_ERROR_RESULT;
    protected readonly ContentParserFactory contentParserFactory;
    protected readonly HTMLSanitizer htmlSanitizer;

    public DataParser(ContentParserFactory contentParserFactory, HTMLSanitizer htmlSanitizer)
    {
        this.contentParserFactory = contentParserFactory;
        this.htmlSanitizer = htmlSanitizer;
        GENERIC_ERROR_RESULT = Result<T>.Error(ErrorMessages.GENERIC_ERROR_MESSAGE);
    }

    public Result<T> ParsePostData(HttpListenerRequest request)
    {
        var contentParser = contentParserFactory.CreateContentParser(request.ContentType);
        if (!contentParser.CanParse(request))
        {
            return GENERIC_ERROR_RESULT;
        }

        var content = contentParser.ParseRequest(request);
        return ExtractDataFromContent(content, request);
    }

    protected abstract Result<T> ExtractDataFromContent(IDictionary<string, ContentData> content, HttpListenerRequest request);
}