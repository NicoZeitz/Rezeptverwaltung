using Core.ValueObjects;
using Server.ContentParser;
using System.Net;

namespace Server.RequestHandler.Login;

public class LoginPostDataParser
{
    private readonly ContentParserFactory contentParserFactory;

    public LoginPostDataParser(ContentParserFactory contentParserFactory)
    {
        this.contentParserFactory = contentParserFactory;
    }

    public LoginData? ParsePostData(HttpListenerRequest request)
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

        return new LoginData(
            new Username(username.TextValue!),
            new Password(password.TextValue!)
        );
    }
}
