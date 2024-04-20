using Core.ValueObjects;
using Server.ContentParser;
using System.Net;

namespace Server.RequestHandler;

public class LoginPostDataParser
{
    private readonly ContentParserFactory contentParserFactory;

    public LoginPostDataParser(ContentParserFactory contentParserFactory)
    {
        this.contentParserFactory = contentParserFactory;
    }

    public LoginPostData? ParsePostData(HttpListenerRequest request)
    {
        var contentParser = contentParserFactory.CreateContentParser(request.ContentType);

        if (!contentParser.CanParse(request))
        {
            return null;
        }

        var content = contentParser.ParseRequest(request);

        if (!content.TryGetValue("username", out var username) && username!.IsText)
        {
            return null;
        }
        if (!content.TryGetValue("password", out var password) && password!.IsText)
        {
            return null;
        }

        return new LoginPostData(
            new Username(username.TextValue!),
            new Password(password.TextValue!)
        );
    }
}
