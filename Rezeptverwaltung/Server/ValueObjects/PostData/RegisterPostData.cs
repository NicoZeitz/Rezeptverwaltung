using Core.ValueObjects;
using Server.ContentParser;

namespace Server.ValueObjects.PostData;

public record struct RegisterPostData(
    Username Username,
    Name Name,
    Password Password,
    Password PasswordRepeat,
    ContentData ProfileImage
);
