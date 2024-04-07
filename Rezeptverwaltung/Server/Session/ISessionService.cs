using Core.Entities;
using System.Net;

namespace Server.Session;

public interface ISessionService
{
    public Chef? GetCurrentChef(HttpListenerRequest request);

    public void Login(HttpListenerRequest request, HttpListenerResponse response, Chef chef);

    public void Logout(HttpListenerRequest request, HttpListenerResponse response);
}