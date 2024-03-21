using Core.ValueObjects;
using Isopoh.Cryptography.Argon2;

// TODO: maybe database project?
namespace Core.Services.Password;

public class Argon2PasswordHasher : IPasswordHasher
{
    public Argon2PasswordHasher() : base() { }

    public HashedPassword HashPassword(ValueObjects.Password password)
    {
        return new HashedPassword(Argon2.Hash(password.Phrase));
    }

    public bool VerifyPassword(ValueObjects.Password password, HashedPassword hashedPassword)
    {
        return Argon2.Verify(hashedPassword.Hash, password.Phrase);
    }
}
