using Core.Data;
using Server.ContentParser;
using System.Net;

namespace Server.DataParser;

public abstract class DataParser<T>
{
    protected readonly Result<T> GENERIC_ERROR_RESULT;
    protected readonly ContentParserFactory contentParserFactory;

    public DataParser(ContentParserFactory contentParserFactory)
    {
        this.contentParserFactory = contentParserFactory;
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
        return ExtractDataFromContent(content);
    }

    protected abstract Result<T> ExtractDataFromContent(IDictionary<string, ContentData> content);
}