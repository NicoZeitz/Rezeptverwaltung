using Core.ValueObjects;
using Server.ContentParser;
using System.Net;

namespace Server.RequestHandler;

public interface SettingsPostData
{
    Task HandlePostRequest(HttpListenerRequest request, HttpListenerResponse response);

    public sealed record class ChangeProfile(
        string? FirstName,
        string? LastName,
        ContentData? ProfileImage
    ) : SettingsPostData
    {
        public Task HandlePostRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            throw new NotImplementedException();
        }
    }

    public sealed record class ChangePassword(
        Password OldPassword,
        Password NewPassword,
        Password NewPasswordRepeated
    ) : SettingsPostData
    {
        public Task HandlePostRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            throw new NotImplementedException();
        }
    }

    public sealed record class DeleteProfile(
        Password Password
    ) : SettingsPostData
    {
        public Task HandlePostRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            throw new NotImplementedException();
        }
    }

}