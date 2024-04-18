using Core.ValueObjects;

namespace Server.RequestHandler.Login;

public record struct LoginData(Username Username, Password Password);