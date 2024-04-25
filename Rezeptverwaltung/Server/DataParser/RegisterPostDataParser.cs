using System.Net;
using Core.Data;
using Core.ValueObjects;
using Server.ContentParser;
using Server.Service;
using Server.ValueObjects.PostData;

namespace Server.DataParser;

public class RegisterPostDataParser(ContentParserFactory contentParserFactory, HTMLSanitizer htmlSanitizer) : DataParser<RegisterPostData>(contentParserFactory, htmlSanitizer)
{
    protected override Result<RegisterPostData> ExtractDataFromContent(IDictionary<string, ContentData> content, HttpListenerRequest request)
    {
        if (!content.TryGetValue("username", out var username) && username!.IsText)
        {
            return GENERIC_ERROR_RESULT;
        }
        if (!content.TryGetValue("first_name", out var firstName) && firstName!.IsText)
        {
            return GENERIC_ERROR_RESULT;
        }
        if (!content.TryGetValue("last_name", out var lastName) && lastName!.IsText)
        {
            return GENERIC_ERROR_RESULT;
        }
        if (!content.TryGetValue("password", out var password) && password!.IsText)
        {
            return GENERIC_ERROR_RESULT;
        }
        if (!content.TryGetValue("password_repeat", out var passwordRepeated) && passwordRepeated!.IsText)
        {
            return GENERIC_ERROR_RESULT;
        }
        if (!content.TryGetValue("profile_image", out var profileImage) && profileImage!.IsFile)
        {
            return GENERIC_ERROR_RESULT;
        }

        if (htmlSanitizer.Sanitize(password.TextValue!) != password.TextValue!)
        {
            return GENERIC_ERROR_RESULT;
        }
        if (htmlSanitizer.Sanitize(passwordRepeated.TextValue!) != password.TextValue)
        {
            return GENERIC_ERROR_RESULT;
        }

        return Result<RegisterPostData>.Successful(new RegisterPostData(
            new Username(htmlSanitizer.Sanitize(username.TextValue!)),
            new Name(htmlSanitizer.Sanitize(firstName.TextValue!), htmlSanitizer.Sanitize(lastName.TextValue!)),
            new Password(htmlSanitizer.Sanitize(password.TextValue!)),
            new Password(htmlSanitizer.Sanitize(passwordRepeated.TextValue!)),
            profileImage!
        ));
    }
}