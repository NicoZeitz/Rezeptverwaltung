using Core.ValueObjects;
using Server.ContentParser;

namespace Server.RequestHandler;

public record struct RegisterPostData(Username Username, Name Name, Password Password, Password PasswordRepeat, ContentData profileImage);
