using Core.Entities;
using Core.Repository;
using Core.Services.Password;
using Core.ValueObjects;

namespace Core.Services;

public sealed class ChefRegisterResult
{
    public readonly bool IsSuccessful;
    public readonly Chef? Chef;
    public readonly IEnumerable<ErrorMessage> ErrorMessages;

    public bool IsError => !IsSuccessful;

    private ChefRegisterResult(bool isSuccessful, Chef? chef, IEnumerable<ErrorMessage> errorMessages)
    {
        IsSuccessful = isSuccessful;
        Chef = chef;
        ErrorMessages = errorMessages;
    }

    public static ChefRegisterResult Successful(Chef chef)
    {
        return new ChefRegisterResult(true, chef, Enumerable.Empty<ErrorMessage>());
    }

    public static ChefRegisterResult Error(IEnumerable<ErrorMessage> errorMessages)
    {
        return new ChefRegisterResult(false, null, errorMessages);
    }
}

public class ChefRegisterService
{
    private readonly ChefRepository chefRepository;
    private readonly IPasswordHasher passwordHasher;
    private readonly AllowedPasswordChecker allowedPasswordChecker;

    public ChefRegisterService(ChefRepository chefRepository, IPasswordHasher passwordHasher, AllowedPasswordChecker allowedPasswordChecker)
    {
        this.chefRepository = chefRepository;
        this.passwordHasher = passwordHasher;
        this.allowedPasswordChecker = allowedPasswordChecker;
    }

    public ChefRegisterResult RegisterChef(Username username, Name name, ValueObjects.Password password)
    {
        var existingChef = chefRepository.findByName(username);
        if (existingChef is not null)
        {
            return ChefRegisterResult.Error(new[] { new ErrorMessage("Benutzername bereits vergeben!") });
        }

        var passwordErrors = allowedPasswordChecker.CheckPassword(password);
        if (passwordErrors.Count() > 0)
        {
            return ChefRegisterResult.Error(passwordErrors);
        }

        var hashedPassword = passwordHasher.HashPassword(password);

        var chef = new Chef(username, name, hashedPassword);
        chefRepository.save(chef);

        return ChefRegisterResult.Successful(chef);
    }
}
