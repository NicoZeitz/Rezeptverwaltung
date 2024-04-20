using System.Net;
using Core.Data;
using Core.ValueObjects;
using Server.ContentParser;

namespace Server.RequestHandler;

public abstract class DataParser<T>
{
    protected readonly ErrorMessage GENERIC_ERROR_MESSAGE = new ErrorMessage("Es ist ein Fehler aufgetreten.");
    protected readonly Result<T> GENERIC_ERROR_RESULT;
    protected readonly ContentParserFactory contentParserFactory;

    public DataParser(ContentParserFactory contentParserFactory)
    {
        this.contentParserFactory = contentParserFactory;
        GENERIC_ERROR_RESULT = Result<T>.Error(GENERIC_ERROR_MESSAGE);
    }

    public Result<T> ParsePostData(HttpListenerRequest request)
    {
        var contentParser = contentParserFactory.CreateContentParser(request.ContentType);
        if (!contentParser.CanParse(request))
        {
            return Result<T>.Error(GENERIC_ERROR_MESSAGE);
        }

        var content = contentParser.ParseRequest(request);
        return ExtractDataFromContent(content);
    }

    public abstract Result<T> ExtractDataFromContent(IDictionary<string, ContentData> content);
}