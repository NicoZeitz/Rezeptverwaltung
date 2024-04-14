using Core.Entities;
using Core.Repository;
using Core.Services.Password;
using Core.ValueObjects;

namespace Core.Services;

public class ChefLoginService
{
    private readonly ChefRepository chefRepository;
    private readonly PasswordHasher passwordHasher;

    public ChefLoginService(ChefRepository chefRepository, PasswordHasher passwordHasher)
    {
        this.chefRepository = chefRepository;
        this.passwordHasher = passwordHasher;
    }

    public Chef? LoginChef(Username username, ValueObjects.Password password)
    {
        var chef = chefRepository.findByName(username);
        if (chef is null)
        {
            return null;
        }

        if (!passwordHasher.VerifyPassword(password, chef.HashedPassword))
        {
            return null;
        };

        return chef;
    }
}
