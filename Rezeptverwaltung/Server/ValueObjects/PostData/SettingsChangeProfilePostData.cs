using Core.Services;
using Core.ValueObjects;
using Server.Components;
using Server.ContentParser;
using Server.Service;
using Server.Session;
using Server.ValueObjects.PostData;
using System.Net;

namespace Server.RequestHandler;

public class SettingsChangeProfilePostData : SettingsPostData
{
    public string? FirstName { get; }
    public string? LastName { get; }
    public ContentData? ProfileImage { get; }

    private readonly ChangeChefDetailsService changeChefDataService;
    private readonly SessionService sessionService;
    private readonly SettingsPageRenderer settingsPageRenderer;
    private readonly ImageTypeMimeTypeConverter imageTypeMimeTypeConverter;

    public SettingsChangeProfilePostData(
        string? firstName,
        string? lastName,
        ContentData? profileImage,
        ChangeChefDetailsService changeChefDataService,
        SessionService sessionService,
        SettingsPageRenderer settingsPageRenderer,
        ImageTypeMimeTypeConverter imageTypeMimeTypeConverter
    )
        : base()
    {
        FirstName = firstName;
        LastName = lastName;
        ProfileImage = profileImage;

        this.changeChefDataService = changeChefDataService;
        this.sessionService = sessionService;
        this.settingsPageRenderer = settingsPageRenderer;
        this.imageTypeMimeTypeConverter = imageTypeMimeTypeConverter;
    }

    public Task HandlePostRequest(HttpListenerRequest request, HttpListenerResponse response)
    {
        var chef = sessionService.GetCurrentChef(request);
        if (chef is null)
        {
            response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return Task.CompletedTask;
        }

        var image = GetImage();
        changeChefDataService.ChangeChefNameAndImage(chef, FirstName, LastName, image);

        return settingsPageRenderer.RenderPage(
            request,
            response,
            chef,
            HttpStatusCode.SeeOther,
            new DisplayableComponent(new Text("Einstellungen erfolgreich geändert!"))
        );
    }

    private Image? GetImage()
    {
        if (ProfileImage is null)
        {
            return null;
        }

        var imageType = imageTypeMimeTypeConverter.ConvertMimeTypeToImageType(ProfileImage.FileMimeType!.Value);
        if (imageType is null)
        {
            return null;
        }

        return new Image(ProfileImage.FileData!, imageType.Value);
    }
}

