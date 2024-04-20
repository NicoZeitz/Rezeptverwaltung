using Core.Data;
using Core.ValueObjects;
using Server.ContentParser;
using System.Net;

namespace Server.RequestHandler;

public class RegisterPostDataParser
{
    private static readonly ErrorMessage GENERIC_ERROR_MESSAGE = new ErrorMessage("Es ist ein Fehler aufgetreten.");
    private readonly ContentParserFactory contentParserFactory;

    public RegisterPostDataParser(ContentParserFactory contentParserFactory)
    {
        this.contentParserFactory = contentParserFactory;
    }

    public Result<RegisterPostData> ParsePostData(HttpListenerRequest request)
    {
        var contentParser = contentParserFactory.CreateContentParser(request.ContentType);
        if (!contentParser.CanParse(request))
        {
            return Result<RegisterPostData>.Error(GENERIC_ERROR_MESSAGE);
        }

        var content = contentParser.ParseRequest(request);

        if (!content.TryGetValue("username", out var username) && username!.IsText)
        {
            return Result<RegisterPostData>.Error(GENERIC_ERROR_MESSAGE);
        }
        if (!content.TryGetValue("first_name", out var firstName) && firstName!.IsText)
        {
            return Result<RegisterPostData>.Error(GENERIC_ERROR_MESSAGE);
        }
        if (!content.TryGetValue("last_name", out var lastName) && lastName!.IsText)
        {
            return Result<RegisterPostData>.Error(GENERIC_ERROR_MESSAGE);
        }
        if (!content.TryGetValue("password", out var password) && password!.IsText)
        {
            return Result<RegisterPostData>.Error(GENERIC_ERROR_MESSAGE);
        }
        if (!content.TryGetValue("password_repeat", out var passwordRepeated) && passwordRepeated!.IsText)
        {
            return Result<RegisterPostData>.Error(GENERIC_ERROR_MESSAGE);
        }
        if (!content.TryGetValue("profile_image", out var profileImage) && profileImage!.IsFile)
        {
            return Result<RegisterPostData>.Error(GENERIC_ERROR_MESSAGE);
        }

        return Result<RegisterPostData>.Successful(new RegisterPostData(
            new Username(username.TextValue!),
            new Name(firstName.TextValue!, lastName.TextValue!),
            new Password(password.TextValue!),
            new Password(passwordRepeated.TextValue!),
            profileImage!
        ));
    }
}