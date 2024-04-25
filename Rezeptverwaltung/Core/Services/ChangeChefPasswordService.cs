using Core.Data;
using Core.Entities;
using Core.Repository;
using Core.Services.Password;
using Core.ValueObjects;

namespace Core.Services;

public class ChangeChefPasswordService
{
    private readonly AllowedPasswordChecker allowedPasswordChecker;
    private readonly ChefRepository chefRepository;
    private readonly DuplicatePasswordChecker duplicatePasswordChecker;
    private readonly PasswordHasher passwordHasher;

    public ChangeChefPasswordService(
        AllowedPasswordChecker allowedPasswordChecker,
        ChefRepository chefRepository,
        DuplicatePasswordChecker duplicatePasswordChecker,
        PasswordHasher passwordHasher)
        : base()
    {
        this.allowedPasswordChecker = allowedPasswordChecker;
        this.chefRepository = chefRepository;
        this.duplicatePasswordChecker = duplicatePasswordChecker;
        this.passwordHasher = passwordHasher;
    }

    public Result<Chef> ChangePassword(Chef chef, ValueObjects.Password password, ValueObjects.Password passwordRepeat, ValueObjects.Password newPassword)
    {
        if (duplicatePasswordChecker.IsSamePassword(password, newPassword))
        {
            return Result<Chef>.Error(new ErrorMessage("Das neue Passwort darf nicht das gleiche sein wie das alte Passwort!"));
        }

        if (!duplicatePasswordChecker.IsSamePassword(newPassword, passwordRepeat))
        {
            return Result<Chef>.Error(new ErrorMessage("Die Passwörter stimmen nicht überein!"));
        }

        if (!passwordHasher.VerifyPassword(password, chef.HashedPassword))
        {
            return Result<Chef>.Error(new ErrorMessage("Falsches Passwort!"));
        }

        var passwordErrors = allowedPasswordChecker.CheckPassword(newPassword);
        if (passwordErrors.Any())
        {
            return Result<Chef>.Errors(passwordErrors);
        }

        var hashedPassword = passwordHasher.HashPassword(newPassword);
        chef.HashedPassword = hashedPassword;
        chefRepository.Update(chef);

        return Result<Chef>.Successful(chef);
    }
}