using Core.Data;
using Core.ValueObjects;
using Server.ContentParser;
using System.Net;

namespace Server.RequestHandler;

public class LoginPostDataParser : DataParser<LoginPostData>
{
    public LoginPostDataParser(ContentParserFactory contentParserFactory)
        : base(contentParserFactory) { }

    public override Result<LoginPostData> ExtractDataFromContent(IDictionary<string, ContentData> content)
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
            new Username(username.TextValue!),
            new Password(password.TextValue!)
        ));
    }
}
