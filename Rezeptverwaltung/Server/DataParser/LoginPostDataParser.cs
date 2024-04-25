using System.Net;
using Core.Data;
using Core.ValueObjects;
using Server.ContentParser;
using Server.RequestHandler;
using Server.Service;

namespace Server.DataParser;

public class LoginPostDataParser(ContentParserFactory contentParserFactory, HTMLSanitizer htmlSanitizer) : DataParser<LoginPostData>(contentParserFactory, htmlSanitizer)
{
    protected override Result<LoginPostData> ExtractDataFromContent(IDictionary<string, ContentData> content, HttpListenerRequest request)
    {
        if (!content.TryGetValue("username", out var username) && username!.IsText)
        {
            return GENERIC_ERROR_RESULT;
        }
        if (!content.TryGetValue("password", out var password) && password!.IsText)
        {
            return GENERIC_ERROR_RESULT;
        }

        return Result<LoginPostData>.Successful(new LoginPostData(
            new Username(htmlSanitizer.Sanitize(username.TextValue!)),
            new Password(htmlSanitizer.Sanitize(password.TextValue!))
        ));
    }
}
