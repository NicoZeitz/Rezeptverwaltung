using Core.ValueObjects;

namespace Server.RequestHandler;

public record struct LoginPostData(Username Username, Password Password);