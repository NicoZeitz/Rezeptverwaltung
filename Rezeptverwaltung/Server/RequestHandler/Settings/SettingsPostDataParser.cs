using Core.Data;
using Core.ValueObjects;
using Server.ContentParser;
using System.Net;

namespace Server.RequestHandler;

public class SettingsPostDataParser
{
    private static readonly ErrorMessage GENERIC_ERROR_MESSAGE = new ErrorMessage("Es ist ein Fehler aufgetreten.");
    private readonly ContentParserFactory contentParserFactory;

    public SettingsPostDataParser(ContentParserFactory contentParserFactory)
    {
        this.contentParserFactory = contentParserFactory;
    }

    public Result<SettingsPostData> ParsePostData(HttpListenerRequest request)
    {
        var contentParser = contentParserFactory.CreateContentParser(request.ContentType);
        if (!contentParser.CanParse(request))
        {
            return Result<SettingsPostData>.Error(GENERIC_ERROR_MESSAGE);
        }

        var content = contentParser.ParseRequest(request);
        if (!content.TryGetValue("type", out var type) && type!.IsText)
        {
            return Result<SettingsPostData>.Error(GENERIC_ERROR_MESSAGE);
        }

        switch (type.TextValue!)
        {
            case "change-data":
                return ParseChangeProfile(content);
            case "change-password":
                return ParseChangePassword(content);
            case "delete-profile":
                return ParseDeleteProfile(content);
            default:
                return Result<SettingsPostData>.Error(GENERIC_ERROR_MESSAGE);
        }
    }
    private Result<SettingsPostData> ParseChangeProfile(IDictionary<string, ContentData> content)
    {
        content.TryGetValue("first_name", out var firstName);
        content.TryGetValue("last_name", out var lastName);
        content.TryGetValue("profile_image", out var profileImage);

        if ((firstName is not null && !firstName.IsText)
            || (lastName is not null && !lastName.IsText)
            || (profileImage is not null && !profileImage.IsFile))
        {
            return Result<SettingsPostData>.Error(GENERIC_ERROR_MESSAGE);
        }

        return Result<SettingsPostData>.Successful(new SettingsPostData.ChangeProfile(
            firstName?.TextValue,
            lastName?.TextValue,
            profileImage
        ));
    }

    private Result<SettingsPostData> ParseChangePassword(IDictionary<string, ContentData> content)
    {
        if (!content.TryGetValue("old_password", out var oldPassword) && oldPassword!.IsText)
        {
            return Result<SettingsPostData>.Error(GENERIC_ERROR_MESSAGE);
        }
        if (!content.TryGetValue("new_password", out var newPassword) && newPassword!.IsText)
        {
            return Result<SettingsPostData>.Error(GENERIC_ERROR_MESSAGE);
        }
        if (!content.TryGetValue("new_password_repeat", out var newPasswordRepeat) && newPasswordRepeat!.IsText)
        {
            return Result<SettingsPostData>.Error(GENERIC_ERROR_MESSAGE);
        }

        return Result<SettingsPostData>.Successful(new SettingsPostData.ChangePassword(
            new Password(oldPassword.TextValue!),
            new Password(newPassword.TextValue!),
            new Password(newPasswordRepeat.TextValue!)
        ));
    }

    private Result<SettingsPostData> ParseDeleteProfile(IDictionary<string, ContentData> content)
    {
        if (!content.TryGetValue("password", out var password) && password!.IsText)
        {
            return Result<SettingsPostData>.Error(GENERIC_ERROR_MESSAGE);
        }

        return Result<SettingsPostData>.Successful(new SettingsPostData.DeleteProfile(
            new Password(password.TextValue!)
        ));
    }
}