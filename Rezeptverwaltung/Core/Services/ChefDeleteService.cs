using Core.Data;
using Core.Entities;
using Core.Repository;
using Core.Services.Password;
using Core.ValueObjects;

namespace Core.Services;

public class ChefDeleteService
{
    private readonly ChefRepository chefRepository;
    private readonly ImageService imageService;
    private readonly PasswordHasher passwordHasher;

    public ChefDeleteService(
        ChefRepository chefRepository,
        ImageService imageService,
        PasswordHasher passwordHasher)
        : base()
    {
        this.chefRepository = chefRepository;
        this.imageService = imageService;
        this.passwordHasher = passwordHasher;
    }

    public Result<Username> DeleteChef(Chef chef, ValueObjects.Password password)
    {
        if (!passwordHasher.VerifyPassword(password, chef.HashedPassword))
        {
            return Result<Username>.Error(new ErrorMessage("Falsches Passwort!"));
        }
        RemoveChefData(chef);
        return Result<Username>.Successful(chef.Username);
    }

    private void RemoveChefData(Chef chef)
    {
        imageService.DeleteImage(chef);
        chefRepository.Remove(chef);
    }
}