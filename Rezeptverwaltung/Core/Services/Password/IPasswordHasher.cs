using Core.ValueObjects;

namespace Core.Services.Password;

public interface IPasswordHasher
{
    HashedPassword HashPassword(ValueObjects.Password password);

    bool VerifyPassword(ValueObjects.Password password, HashedPassword hash);
}
