namespace Server;

public readonly struct HttpStatus : IEquatable<HttpStatus>
{
    public static readonly HttpStatus OK = new HttpStatus(200, "OK");
    public static readonly HttpStatus CREATED = new HttpStatus(201, "Created");
    public static readonly HttpStatus NO_CONTENT = new HttpStatus(204, "No Content");
    public static readonly HttpStatus FOUND = new HttpStatus(302, "Found");
    public static readonly HttpStatus SEE_OTHER = new HttpStatus(303, "See Other");
    public static readonly HttpStatus BAD_REQUEST = new HttpStatus(400, "Bad Request");
    public static readonly HttpStatus UNAUTHORIZED = new HttpStatus(401, "Unauthorized");
    public static readonly HttpStatus FORBIDDEN = new HttpStatus(403, "Forbidden");
    public static readonly HttpStatus NOT_FOUND = new HttpStatus(404, "Not Found");
    public static readonly HttpStatus INTERNAL_SERVER_ERROR = new HttpStatus(500, "Internal Server Error");

    public readonly int Code;
    public readonly string Description;

    private HttpStatus(int code, string description)
    {
        Code = code;
        Description = description;
    }

    public bool Equals(HttpStatus other) => Code == other.Code && Description == other.Description;

    public override int GetHashCode() => HashCode.Combine(Code.GetHashCode(), Description.GetHashCode());

    public override bool Equals(object? obj) => obj is HttpStatus httpStatus && Equals(httpStatus);

    public static bool operator ==(HttpStatus left, HttpStatus right) => left.Equals(right);

    public static bool operator !=(HttpStatus left, HttpStatus right) => !(left == right);
}
