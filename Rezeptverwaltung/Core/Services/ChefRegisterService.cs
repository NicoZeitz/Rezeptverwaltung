using Core.Data;
using Core.Entities;
using Core.Repository;
using Core.Services.Password;
using Core.ValueObjects;

namespace Core.Services;

public class ChefRegisterService
{
    private readonly ChefRepository chefRepository;
    private readonly PasswordHasher passwordHasher;
    private readonly AllowedPasswordChecker allowedPasswordChecker;

    public ChefRegisterService(ChefRepository chefRepository, PasswordHasher passwordHasher, AllowedPasswordChecker allowedPasswordChecker)
    {
        this.chefRepository = chefRepository;
        this.passwordHasher = passwordHasher;
        this.allowedPasswordChecker = allowedPasswordChecker;
    }

    public Result<Chef> RegisterChef(Username username, Name name, ValueObjects.Password password, Image image)
    {
        var existingChef = chefRepository.FindByUsername(username);
        if (existingChef is not null)
        {
            return Result<Chef>.Error(new[] { new ErrorMessage("Benutzername bereits vergeben!") });
        }

        var passwordErrors = allowedPasswordChecker.CheckPassword(password);
        if (passwordErrors.Count() > 0)
        {
            return Result<Chef>.Error(passwordErrors);
        }

        var hashedPassword = passwordHasher.HashPassword(password);

        var chef = new Chef(username, name, hashedPassword);
        chefRepository.Add(chef);

        return Result<Chef>.Successful(chef);
    }
}
