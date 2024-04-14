using Core.ValueObjects;

namespace Core.Services.Password;

public interface PasswordHasher
{
    HashedPassword HashPassword(ValueObjects.Password password);

    bool VerifyPassword(ValueObjects.Password password, HashedPassword hash);
}
