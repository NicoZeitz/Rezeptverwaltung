using Core.Data;
using Core.Services;
using Core.ValueObjects;
using Server.ContentParser;
using Server.Service;
using Server.Session;
using System.Net;

namespace Server.RequestHandler;

public class SettingsPostDataParser
{
    private static readonly ErrorMessage GENERIC_ERROR_MESSAGE = new ErrorMessage("Es ist ein Fehler aufgetreten.");

    private readonly ChefChangePasswordService chefChangePasswordService;
    private readonly ChefDeleteService chefDeleteService;
    private readonly ChefChangeDataService chefChangeDataService;
    private readonly ImageTypeMimeTypeConverter imageTypeMimeTypeConverter;
    private readonly ContentParserFactory contentParserFactory;
    private readonly SessionService sessionService;
    private readonly SettingsPageRenderer settingsPageRenderer;


    public SettingsPostDataParser(
        ChefChangePasswordService chefChangePasswordService,
        ContentParserFactory contentParserFactory,
        SessionService sessionService,
        SettingsPageRenderer settingsPageRenderer,
        ChefDeleteService chefDeleteService,
        ChefChangeDataService chefChangeDataService,
        ImageTypeMimeTypeConverter imageTypeMimeTypeConverter)
        : base()
    {
        this.chefChangePasswordService = chefChangePasswordService;
        this.chefDeleteService = chefDeleteService;
        this.chefChangeDataService = chefChangeDataService;
        this.imageTypeMimeTypeConverter = imageTypeMimeTypeConverter;
        this.contentParserFactory = contentParserFactory;
        this.sessionService = sessionService;
        this.settingsPageRenderer = settingsPageRenderer;
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

        if ((firstName is not null && !firstName.IsText) ||
           (lastName is not null && !lastName.IsText) ||
           (profileImage is not null && !profileImage.IsFile))
        {
            return Result<SettingsPostData>.Error(GENERIC_ERROR_MESSAGE);
        }

        return Result<SettingsPostData>.Successful(new SettingsChangeProfilePostData(
            firstName?.TextValue,
            lastName?.TextValue,
            profileImage,
            chefChangeDataService,
            sessionService,
            settingsPageRenderer,
            imageTypeMimeTypeConverter
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

        return Result<SettingsPostData>.Successful(new SettingsChangePasswordPostData(
            new Password(oldPassword.TextValue!),
            new Password(newPassword.TextValue!),
            new Password(newPasswordRepeat.TextValue!),
            chefChangePasswordService,
            sessionService,
            settingsPageRenderer
        ));
    }

    private Result<SettingsPostData> ParseDeleteProfile(IDictionary<string, ContentData> content)
    {
        if (!content.TryGetValue("password", out var password) && password!.IsText)
        {
            return Result<SettingsPostData>.Error(GENERIC_ERROR_MESSAGE);
        }

        return Result<SettingsPostData>.Successful(new SettingsDeleteProfilePostData(
            new Password(password.TextValue!),
            chefDeleteService,
            sessionService,
            settingsPageRenderer
        ));
    }
}