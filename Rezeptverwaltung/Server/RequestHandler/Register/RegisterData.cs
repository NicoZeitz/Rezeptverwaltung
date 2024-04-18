using Core.ValueObjects;
using Server.ContentParser;

namespace Server.RequestHandler.Register;

public record struct RegisterData(Username Username, Name Name, Password Password, Password PasswordRepeat, ContentData profileImage);
