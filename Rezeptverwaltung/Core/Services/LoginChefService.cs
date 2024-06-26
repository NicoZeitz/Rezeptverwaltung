﻿using Core.Entities;
using Core.Repository;
using Core.Services.Password;
using Core.ValueObjects;

namespace Core.Services;

public class LoginChefService
{
    private readonly ChefRepository chefRepository;
    private readonly PasswordHasher passwordHasher;

    public LoginChefService(ChefRepository chefRepository, PasswordHasher passwordHasher)
    {
        this.chefRepository = chefRepository;
        this.passwordHasher = passwordHasher;
    }

    public Chef? LoginChef(Username username, ValueObjects.Password password)
    {
        var chef = chefRepository.FindByUsername(username);
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
