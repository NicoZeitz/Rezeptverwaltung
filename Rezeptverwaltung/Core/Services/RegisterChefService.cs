using Core.Data;
using Core.Entities;
using Core.Repository;
using Core.Services.Password;
using Core.ValueObjects;

namespace Core.Services;

public class RegisterChefService
{
    private readonly ChefRepository chefRepository;
    private readonly PasswordHasher passwordHasher;
    private readonly AllowedPasswordChecker allowedPasswordChecker;
    private readonly ImageService imageService;

    public RegisterChefService(
        ChefRepository chefRepository,
        PasswordHasher passwordHasher,
        AllowedPasswordChecker allowedPasswordChecker,
        ImageService imageService)
        : base()
    {
        this.chefRepository = chefRepository;
        this.passwordHasher = passwordHasher;
        this.allowedPasswordChecker = allowedPasswordChecker;
        this.imageService = imageService;
    }

    public Result<Chef> RegisterChef(Username username, Name name, ValueObjects.Password password, Image image)
    {
        var existingChef = chefRepository.FindByUsername(username);
        if (existingChef is not null)
        {
            return Result<Chef>.Error(new ErrorMessage("Benutzername bereits vergeben!"));
        }

        var passwordErrors = allowedPasswordChecker.CheckPassword(password);
        if (passwordErrors.Any())
        {
            return Result<Chef>.Errors(passwordErrors);
        }

        var hashedPassword = passwordHasher.HashPassword(password);

        var chef = new Chef(username, name, hashedPassword);

        chefRepository.Add(chef);
        imageService.SaveImage(chef, image);

        return Result<Chef>.Successful(chef);
    }
}
