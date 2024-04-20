using Core.Services;
using Core.ValueObjects;
using Server.Components;
using Server.ContentParser;
using Server.Service;
using Server.Session;
using System.Net;

namespace Server.RequestHandler;

public class SettingsChangeProfilePostData : SettingsPostData
{
    public string? FirstName { get; }
    public string? LastName { get; }
    public ContentData? ProfileImage { get; }

    private readonly ChefChangeDataService chefChangeDataService;
    private readonly SessionService sessionService;
    private readonly SettingsPageRenderer settingsPageRenderer;
    private readonly ImageTypeMimeTypeConverter imageTypeMimeTypeConverter;

    public SettingsChangeProfilePostData(
        string? firstName,
        string? lastName,
        ContentData? profileImage,
        ChefChangeDataService chefChangeDataService,
        SessionService sessionService,
        SettingsPageRenderer settingsPageRenderer,
        ImageTypeMimeTypeConverter imageTypeMimeTypeConverter
    )
        : base()
    {
        FirstName = firstName;
        LastName = lastName;
        ProfileImage = profileImage;

        this.chefChangeDataService = chefChangeDataService;
        this.sessionService = sessionService;
        this.settingsPageRenderer = settingsPageRenderer;
        this.imageTypeMimeTypeConverter = imageTypeMimeTypeConverter;
    }

    public Task HandlePostRequest(HttpListenerRequest request, HttpListenerResponse response)
    {
        var chef = sessionService.GetCurrentChef(request);
        if (chef is null)
        {
            return settingsPageRenderer.RenderPage(
                request,
                response,
                HttpStatusCode.BadRequest,
                new Dictionary<string, IEnumerable<ErrorMessage>>()
            );
        }

        var image = GetImage();
        chefChangeDataService.ChangeData(chef, FirstName, LastName, image);

        return settingsPageRenderer.RenderPage(
            request,
            response,
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
        return new Image(ProfileImage.FileData!, imageType!.Value);
    }
}

