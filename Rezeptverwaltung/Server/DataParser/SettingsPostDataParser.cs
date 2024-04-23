using System.Net;
using Core.Data;
using Core.Services;
using Core.ValueObjects;
using Server.ContentParser;
using Server.RequestHandler;
using Server.Service;
using Server.Session;
using Server.ValueObjects.PostData;

namespace Server.DataParser;

public class SettingsPostDataParser : DataParser<SettingsPostData>
{
    private readonly ChangeChefPasswordService changeChefPasswordService;
    private readonly DeleteChefService chefDeleteService;
    private readonly ChangeChefDataService changeChefDataService;
    private readonly ImageTypeMimeTypeConverter imageTypeMimeTypeConverter;
    private readonly SessionService sessionService;
    private readonly SettingsPageRenderer settingsPageRenderer;
    private readonly RedirectService redirectService;

    public SettingsPostDataParser(
        ChangeChefPasswordService changeChefPasswordService,
        ContentParserFactory contentParserFactory,
        SessionService sessionService,
        SettingsPageRenderer settingsPageRenderer,
        DeleteChefService chefDeleteService,
        ChangeChefDataService changeChefDataService,
        ImageTypeMimeTypeConverter imageTypeMimeTypeConverter,
        RedirectService redirectService)
        : base(contentParserFactory)
    {
        this.changeChefPasswordService = changeChefPasswordService;
        this.chefDeleteService = chefDeleteService;
        this.changeChefDataService = changeChefDataService;
        this.imageTypeMimeTypeConverter = imageTypeMimeTypeConverter;
        this.sessionService = sessionService;
        this.settingsPageRenderer = settingsPageRenderer;
        this.redirectService = redirectService;
    }

    protected override Result<SettingsPostData> ExtractDataFromContent(IDictionary<string, ContentData> content, HttpListenerRequest request)
    {
        if (!content.TryGetValue("type", out var type) && type!.IsText)
        {
            return GENERIC_ERROR_RESULT;
        }

        return type.TextValue! switch
        {
            "change-data" => ParseChangeProfile(content),
            "change-password" => ParseChangePassword(content),
            "delete-profile" => ParseDeleteProfile(content),
            _ => GENERIC_ERROR_RESULT,
        };
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
            return GENERIC_ERROR_RESULT;
        }

        return Result<SettingsPostData>.Successful(new SettingsChangeProfilePostData(
            firstName?.TextValue,
            lastName?.TextValue,
            profileImage,
            changeChefDataService,
            sessionService,
            settingsPageRenderer,
            imageTypeMimeTypeConverter
        ));
    }

    private Result<SettingsPostData> ParseChangePassword(IDictionary<string, ContentData> content)
    {
        if (!content.TryGetValue("old_password", out var oldPassword) && oldPassword!.IsText)
        {
            return GENERIC_ERROR_RESULT;
        }
        if (!content.TryGetValue("new_password", out var newPassword) && newPassword!.IsText)
        {
            return GENERIC_ERROR_RESULT;
        }
        if (!content.TryGetValue("new_password_repeat", out var newPasswordRepeat) && newPasswordRepeat!.IsText)
        {
            return GENERIC_ERROR_RESULT;
        }

        return Result<SettingsPostData>.Successful(new SettingsChangePasswordPostData(
            new Password(oldPassword.TextValue!),
            new Password(newPassword.TextValue!),
            new Password(newPasswordRepeat.TextValue!),
            changeChefPasswordService,
            sessionService,
            settingsPageRenderer
        ));
    }

    private Result<SettingsPostData> ParseDeleteProfile(IDictionary<string, ContentData> content)
    {
        if (!content.TryGetValue("password", out var password) && password!.IsText)
        {
            return GENERIC_ERROR_RESULT;
        }

        return Result<SettingsPostData>.Successful(new SettingsDeleteProfilePostData(
            new Password(password.TextValue!),
            chefDeleteService,
            sessionService,
            settingsPageRenderer,
            redirectService
        ));
    }
}